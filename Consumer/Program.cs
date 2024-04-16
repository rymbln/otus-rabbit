
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Serilog;

using System.Text;

Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();


var factory = new ConnectionFactory { 
    HostName = Environment.GetEnvironmentVariable("RABBIT_HOST"), 
    UserName = Environment.GetEnvironmentVariable("RABBIT_USER"), 
    Password = Environment.GetEnvironmentVariable("RABBIT_PASS")
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var queue = Environment.GetEnvironmentVariable("RABBIT_QUEUE") ?? string.Empty;
var exchange = Environment.GetEnvironmentVariable("RABBIT_EXCHANGE") ?? string.Empty;
var routingKey = Environment.GetEnvironmentVariable("RABBIT_ROUTING") ?? string.Empty;

// 1. Queue
if (exchange == "" && queue != "" && routingKey == "")
{
    // Если имя очереди задано, работаем с этой очередью
    channel.QueueDeclare(queue: queue,
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);
}
// 2. Publish/Subscribe
else if (exchange != "" && queue == "" && routingKey == "")
{
    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);
    queue = channel.QueueDeclare().QueueName;
    channel.QueueBind(queue: queue, 
                      exchange: exchange, 
                      routingKey: routingKey);
}
// 3. Routing
else if (exchange != "" && queue == "" && routingKey != "" && (!routingKey.Contains(".") && !routingKey.Contains("#")))
{
    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);
    queue = channel.QueueDeclare().QueueName;
    channel.QueueBind(queue: queue,
                  exchange: exchange,
                  routingKey: routingKey);
}
// 4. Topic
else if (exchange != "" && queue == "" && routingKey != "" && (routingKey.Contains(".") || routingKey.Contains("#")))
{
    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);
    queue = channel.QueueDeclare().QueueName;
    channel.QueueBind(queue: queue,
                  exchange: exchange,
                  routingKey: routingKey);
} else
{
    Log.Error("Alarm");
    return;
}

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Log.Information(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Log.Information($" [x] Received message '{message}' From Exchange: '{exchange}' Queue: '{queue}' Routing: '{routingKey}'");

    int dots = message.Split('.').Length;
    Thread.Sleep(dots * 1000);

    Log.Information($" [x] Processed message '{message}' From Exchange: '{exchange}' Queue: '{queue}' Routing: '{routingKey}'");

    // here channel could also be accessed as ((EventingBasicConsumer)sender).Model
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};
channel.BasicConsume(queue: queue,
                     autoAck: false,
                     consumer: consumer);

Log.Information(" Press [enter] to exit.");
Console.ReadLine();
