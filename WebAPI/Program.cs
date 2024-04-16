using MassTransit;
using MassTransit.Transports;

using Microsoft.AspNetCore.Http.HttpResults;

using RabbitMQ.Client;

using Serilog;

using SharedClassLibrary;

using System.Text;

using WebAPI;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
var logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();
builder.Logging.AddSerilog(logger);

builder.Services.AddSingleton<IRabbitMqProducer, RabbitMqProducer>();

builder.Services.AddMassTransit(x =>
{
    x.AddDelayedMessageScheduler();
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
        Environment.GetEnvironmentVariable("RABBIT_HOST"),
        5672,
        "/", h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBIT_USER"));
            h.Password(Environment.GetEnvironmentVariable("RABBIT_PASS"));
            h.Heartbeat(30);
        });

    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/rabbit", (RabbitMessageRouting msg, IRabbitMqProducer rabbit) =>
{
    rabbit.SendTextMessage(msg.Body, msg.Exchange, msg.RoutingKey);
    return "Message sended";
})
.WithName("SendRabbitMessage")
.WithOpenApi();

app.MapPost("/rabbit/rpc", async (RabbitMessage msg, IRabbitMqProducer rabbit) =>
{
    var res = await rabbit.CallRpc(msg.Body);
    return res;
})
.WithName("SendRabbitRpc")
.WithOpenApi();

app.MapPost("/masstransit", async (MassTransitMessageDto dto, IPublishEndpoint pub) =>
{
    await pub.Publish<MessageCreated>(new
    {
        Id = 1,
        dto.From,
        dto.To,
        dto.Message
    });
    return "Message sended";
})
.WithName("SendMessTransitMessage")
.WithOpenApi();

app.Run();

internal record RabbitMessageRouting(string Exchange = "", string RoutingKey = "", string Body = "") {  }
internal record RabbitMessage(string Body = "") { }
internal record MassTransitMessageDto(string From = "", string To = "", string Message = "") { }