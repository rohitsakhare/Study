using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;

namespace Shared.Messaging.Kafka;

public class KafkaTopicInitializer(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public async Task EnsureTopicsExistAsync()
    {
        var bootstrapServers = _config["Kafka:BootstrapServers"];

        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        }).Build();

        var requiredTopics = new List<TopicSpecification>
        {
            new TopicSpecification { Name = "product-events", NumPartitions = 3, ReplicationFactor = 3 },
            // new TopicSpecification { Name = "order-events", NumPartitions = 3, ReplicationFactor = 3 },
            // new TopicSpecification { Name = "payment-events", NumPartitions = 3, ReplicationFactor = 3 },
            // new TopicSpecification { Name = "delivery-events", NumPartitions = 3, ReplicationFactor = 3 },
            // new TopicSpecification { Name = "notification-events", NumPartitions = 3, ReplicationFactor = 3 }
        };

        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
        var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet();

        var topicsToCreate = requiredTopics
            .Where(t => !existingTopics.Contains(t.Name))
            .ToList();

        if (topicsToCreate.Count > 0)
        {
            try
            {
                await adminClient.CreateTopicsAsync(topicsToCreate);
            }
            catch (CreateTopicsException ex)
            {
                foreach (var result in ex.Results)
                {
                    Console.WriteLine($"Topic creation error: {result.Error.Reason}");
                }
            }
        }
    }
}
