using Confluent.Kafka;
using Newtonsoft.Json;
using System.Text;

namespace Shared.EventBus;

public class KafkaProducer<T>
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaProducer(string brokerList, string topic)
    {
        var config = new ProducerConfig { BootstrapServers = brokerList };
        _producer = new ProducerBuilder<Null, string>(config).Build();
        _topic = topic;
    }

    public async Task PublishAsync(T message)
    {
        var json = JsonConvert.SerializeObject(message);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json });
    }
}
