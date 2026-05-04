namespace Shared.Messaging.Abstractions;

public interface IEventBus
{
    Task PublishAsync<T>(string topic, T message);
}
