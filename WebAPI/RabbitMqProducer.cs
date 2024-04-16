using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace WebAPI
{
    public interface IRabbitMqProducer
    {
        void SendTextMessage(string msg, string exchange = "", string routingKey = "");
        Task<string> CallRpc(string msg);
    }

    public class RabbitMqProducer : IRabbitMqProducer, IDisposable
    {
        private readonly ILogger<RabbitMqProducer> _logger;
        private readonly IConnection _connection;
        public RabbitMqProducer(ILogger<RabbitMqProducer> logger)
        {
            _logger = logger;
            
            var factory = new ConnectionFactory { 
                HostName = Environment.GetEnvironmentVariable("RABBIT_HOST"), 
                UserName = Environment.GetEnvironmentVariable("RABBIT_USER"), 
                Password = Environment.GetEnvironmentVariable("RABBIT_PASS")
            };
            _connection = factory.CreateConnection();
            
        }

        public void SendTextMessage(string msg, string exchange = "", string routingKey = "")
        {
            using var channel = _connection.CreateModel();

            // 1. Queue
            if (exchange == "" && routingKey != "")
            {
                channel.QueueDeclare(queue: routingKey,
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);
            }
            // 2. Publish/Subscribe
            else if (exchange != "" && routingKey == "")
            {
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);
            }
            // 3. Routing
            else if (exchange != "" && routingKey != "" && !routingKey.Contains("."))
            {
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);
            }
            // 4. Topic
            else if (exchange != "" && routingKey != "" && routingKey.Contains("."))
            {
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);
            }


            var body = Encoding.UTF8.GetBytes(msg);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: properties,
                                 body: body);

            _logger.LogInformation($" [x] Message '{msg}' sent to Exchange '{exchange}' with routing key '{routingKey}'");
        }

        public async Task<string> CallRpc(string msg)
        {
            using var rpcClient = new RpcClient(Environment.GetEnvironmentVariable("RABBIT_RPC_QUEUE"), _connection);
            var response = await rpcClient.CallAsync(msg);
            _logger.LogInformation(response);
            return response;


        }
        public void Dispose() {

            _connection.Dispose();
        }
    }
}
