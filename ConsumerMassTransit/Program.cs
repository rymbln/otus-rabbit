// See https://aka.ms/new-console-template for more information
using ConsumerMassTransit;

using MassTransit;

using Microsoft.Extensions.Logging;

using Serilog;


Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

//using var loggerFactory = LoggerFactory.Create(builder =>
//{
//    builder
//        .AddFilter("Microsoft", LogLevel.Warning)
//        .AddFilter("System", LogLevel.Warning)
//        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
//        .AddConsole();
//});
var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
{
    x.Host(Environment.GetEnvironmentVariable("RABBIT_HOST"),
        5672,"/", 
        h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBIT_USER"));
            h.Password(Environment.GetEnvironmentVariable("RABBIT_PASS"));
            h.Heartbeat(30);
        });
    x.ReceiveEndpoint("message-created-event", e =>
    {
        e.Consumer<MessageCreatedConsumer>();
    });
    //LogContext.ConfigureCurrentLogContext(loggerFactory);
});

await busControl.StartAsync(new CancellationToken());
try
{
    Log.Information("Press enter to exit");
    await Task.Run(() => Console.ReadLine());
}
finally
{
    await busControl.StopAsync();
}