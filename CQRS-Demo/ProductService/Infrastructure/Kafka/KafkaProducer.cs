using Confluent.Kafka;

namespace ProductService.Infrastructure.Kafka;

public class KafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration config)
    {
        _producer = new ProducerBuilder<string, string>(
            new ProducerConfig 
            { 
                BootstrapServers = config["Kafka:BootstrapServers"] ,
                EnableIdempotence = true,
                Acks = Acks.All,
                LingerMs = 5,
                BatchSize = 16384
            }).Build();
    }

    public Task Send(string topic, string message) => _producer.ProduceAsync(topic, new Message<string, string> { Value = message });
}