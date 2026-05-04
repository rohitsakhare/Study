// Sample Event Publishing/Subscribing with RabbitMQ
// Add this to implement event-driven architecture:

using Shared.Messaging;
using RabbitMQ.Client;
using System.Text.Json;

namespace UserService.Application.Services;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqEventPublisher(string connectionString)
    {
        var factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task PublishAsync<T>(T @event) where T : IMessage
    {
        var eventName = @event.GetType().Name;
        _channel.ExchangeDeclare(exchange: "saas-events", type: ExchangeType.Topic, durable: true);
        
        var json = JsonSerializer.Serialize(@event);
        var body = System.Text.Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        _channel.BasicPublish(
            exchange: "saas-events",
            routingKey: eventName,
            basicProperties: properties,
            body: body);

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}

// Usage in UserService:
public class UserCreatedEventPublisher
{
    private readonly IEventPublisher _eventPublisher;

    public UserCreatedEventPublisher(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task PublishUserCreatedEvent(int userId, string username, string email)
    {
        var @event = new UserCreatedEvent
        {
            UserId = userId,
            Username = username,
            Email = email
        };

        await _eventPublisher.PublishAsync(@event);
    }
}

// Register in Program.cs:
// builder.Services.AddSingleton<IEventPublisher>(sp =>
//     new RabbitMqEventPublisher("amqp://guest:guest@rabbitmq:5672/"));
