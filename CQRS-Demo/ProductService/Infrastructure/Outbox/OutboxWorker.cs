using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Kafka;

namespace ProductService.Infrastructure.Outbox;

public class OutboxWorker(IServiceScopeFactory scopeFactory, KafkaProducer producer) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly KafkaProducer _producer = producer;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

            var messages = await db.OutboxMessages
                .Where(x => !x.Processed)
                .Take(10)
                .ToListAsync(cancellationToken: ct);

            foreach (var msg in messages)
            {
                await _producer.Send("product-events", msg.Content);
                msg.Processed = true;
            }

            await db.SaveChangesAsync(ct);
            await Task.Delay(2000, ct);
        }
    }
}