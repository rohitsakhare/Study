using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Messaging.Abstractions;

namespace Shared.Outbox;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IOutboxDbContext>();
            var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();

            var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IOutboxHandler>>().ToDictionary(x => x.EventType);

            var messages = await db.OutboxMessages.Where(x => x.Status == "Pending" && x.RetryCount < 5).OrderBy(x => x.OccurredOn).Take(20).ToListAsync(stoppingToken);

            foreach (var msg in messages)
            {
                try
                {
                    msg.Status = "Processing";
                    await db.SaveChangesAsync(stoppingToken);

                    await Dispatch(msg, bus, handlers);

                    msg.Status = "Processed";
                    msg.ProcessedOn = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    msg.RetryCount++;
                    msg.LastError = ex.Message;

                    msg.Status = msg.RetryCount >= 5 ? "Dead" : "Pending";
                }
            }

            await db.SaveChangesAsync(stoppingToken);

            await Task.Delay(2000, stoppingToken);
        }
    }



    private static async Task Dispatch(OutboxMessage msg, IEventBus bus, Dictionary<string, IOutboxHandler> handlers)
    {
        if (!handlers.TryGetValue(msg.Type, out var handler))
        {
            throw new InvalidOperationException($"No outbox handler registered for event type '{msg.Type}'.");
        }

        await handler.Handle(msg.Payload, bus);
    }
}