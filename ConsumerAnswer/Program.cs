// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Serilog;

using System.Text;

Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

var factory = new ConnectionFactory
{
    HostName = Environment.GetEnvironmentVariable("RABBIT_HOST"),
    UserName = Environment.GetEnvironmentVariable("RABBIT_USER"),
    Password = Environment.GetEnvironmentVariable("RABBIT_PASS")
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var queue = Environment.GetEnvironmentVariable("RABBIT_RPC_QUEUE");

channel.QueueDeclare(queue: queue,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume(queue: queue,
                     autoAck: false,
                     consumer: consumer);
Log.Information(" [x] Awaiting RPC requests");

consumer.Received += (model, ea) =>
{
    string response = string.Empty;

    var body = ea.Body.ToArray();
    var props = ea.BasicProperties;
    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = props.CorrelationId;

    try
    {
        var message = Encoding.UTF8.GetString(body);
        Log.Information($" [x] Received message {message}");

        int dots = message.Split('.').Length - 1;
        Thread.Sleep(dots * 1000);

        Log.Information($" [x] Processed message {message}");

        response = $"Message contains {dots} dots";
    }
    catch (Exception e)
    {
        Log.Information($" [.] {e.Message}");
        response = string.Empty;
    }
    finally
    {
        var responseBytes = Encoding.UTF8.GetBytes(response);
        channel.BasicPublish(exchange: string.Empty,
                             routingKey: props.ReplyTo,
                             basicProperties: replyProps,
                             body: responseBytes);
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }
};

Log.Information(" Press [enter] to exit.");
Console.ReadLine();