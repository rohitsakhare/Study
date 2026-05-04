namespace Shared.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event) where T : IMessage;
}

public interface IEventSubscriber
{
    Task SubscribeAsync<T>(Func<T, Task> handler) where T : IMessage;
}

// RabbitMQ Implementation Placeholder
public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly string _connectionString;

    public RabbitMqEventPublisher(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task PublishAsync<T>(T @event) where T : IMessage
    {
        // RabbitMQ implementation
        // This is a placeholder - in production, use RabbitMQ.Client to publish messages
        await Task.CompletedTask;
    }
}

public class RabbitMqEventSubscriber : IEventSubscriber
{
    private readonly string _connectionString;

    public RabbitMqEventSubscriber(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task SubscribeAsync<T>(Func<T, Task> handler) where T : IMessage
    {
        // RabbitMQ implementation
        // This is a placeholder - in production, use RabbitMQ.Client to subscribe to messages
        await Task.CompletedTask;
    }
}
