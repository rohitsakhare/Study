using Confluent.Kafka;
using Newtonsoft.Json;

namespace Shared.EventBus;

public class KafkaConsumer<T>
{
    private readonly string _brokerList;
    private readonly string _topic;
    private readonly string _groupId;

    public KafkaConsumer(string brokerList, string topic, string groupId)
    {
        _brokerList = brokerList;
        _topic = topic;
        _groupId = groupId;
    }

    public void StartConsuming(Func<T, Task> handleMessage)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _brokerList,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_topic);

        Task.Run(() =>
        {
            while (true)
            {
                var cr = consumer.Consume();
                var message = JsonConvert.DeserializeObject<T>(cr.Message.Value);
                handleMessage(message);
            }
        });
    }
}
