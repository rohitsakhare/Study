using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Shared.Messaging.Abstractions;
using System.Text.Json;

namespace Shared.Messaging.Kafka;

public class EventBus : IEventBus, IDisposable
{
    private readonly IProducer<string, string> _producer;

    public EventBus(IConfiguration config)
    {
        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"],
            EnableIdempotence = true, // 🔥 important for reliability
            Acks = Acks.All,
            LingerMs = 5,
            BatchSize = 16384
        };
        _producer = new ProducerBuilder<string, string>(kafkaConfig).Build();
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        var json = JsonSerializer.Serialize(message);

        await _producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = json
        });
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}