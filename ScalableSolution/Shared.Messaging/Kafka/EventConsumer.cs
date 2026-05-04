using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using Shared.Messaging.Abstractions;
using System.Text.Json;

namespace Shared.Messaging.Kafka;

public class EventConsumer<T> : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _topic;

    public EventConsumer(IConfiguration config, IServiceScopeFactory scopeFactory, string topic)
    {
        _scopeFactory = scopeFactory;
        _topic = topic;

        var conf = new ConsumerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"],
            GroupId = config["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 10000

        };
        _consumer = new ConsumerBuilder<string, string>(conf).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);

        return Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);

                using var jsonDoc = JsonDocument.Parse(result.Message.Value);
                if (!jsonDoc.RootElement.TryGetProperty("EventType", out var eventTypeProperty) ||
                    eventTypeProperty.GetString() != typeof(T).Name)
                {
                    continue;
                }

                if (!jsonDoc.RootElement.TryGetProperty("EventId", out var eventIdProperty) ||
                    !Guid.TryParse(eventIdProperty.GetString(), out var eventId))
                {
                    continue;
                }

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IInboxDbContext>();

                var inboxEntry = await db.InboxMessages.FirstOrDefaultAsync(x => x.EventId == eventId, stoppingToken);
                if (inboxEntry?.Status == "Processed")
                    continue;

                if (inboxEntry == null)
                {
                    inboxEntry = new InboxMessage
                    {
                        Id = Guid.NewGuid(),
                        EventId = eventId,
                        EventType = typeof(T).Name,
                        Payload = result.Message.Value,
                        ReceivedOn = DateTime.UtcNow,
                        Status = "Processing"
                    };
                    db.InboxMessages.Add(inboxEntry);
                    await db.SaveChangesAsync(stoppingToken);
                }
                else if (inboxEntry.Status != "Processing")
                {
                    inboxEntry.Status = "Processing";
                    await db.SaveChangesAsync(stoppingToken);
                }

                var handler = scope.ServiceProvider.GetRequiredService<IEventConsumer<T>>();
                var message = JsonSerializer.Deserialize<T>(result.Message.Value);

                try
                {
                    await handler.ConsumeAsync(message!, stoppingToken);
                    inboxEntry.Status = "Processed";
                    inboxEntry.ProcessedOn = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    inboxEntry.Status = "Pending";
                    inboxEntry.RetryCount++;
                    inboxEntry.LastError = ex.Message;
                    await db.SaveChangesAsync(stoppingToken);
                    throw;
                }
            }
        }, stoppingToken);
    }
}
