using MassTransit;
using MassTransit.Transports;

using Microsoft.Extensions.Logging;

using Serilog;

using SharedClassLibrary;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsumerMassTransit
{
    public class MessageCreatedConsumer: IConsumer<MessageCreated>
    {
        public async Task Consume(ConsumeContext<MessageCreated> context)
        {
            var msg = context.Message;
            var jsonMessage = JsonSerializer.Serialize(msg);
            await Task.Delay(1000);
            Log.Information($"Message processed: {jsonMessage}");
           
        }

    }
}
