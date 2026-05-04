namespace Shared.Messaging.Abstractions;

public interface IEventConsumer<T>
{
    Task ConsumeAsync(T message, CancellationToken cancellationToken);
}