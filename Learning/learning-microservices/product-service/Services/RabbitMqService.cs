using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace product_service.Services;

public class RabbitMqService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            UserName = "admin",
            Password = "admin"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare a queue for product events
        _channel.QueueDeclare(queue: "product_events",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    public void PublishProductEvent(string eventType, object product)
    {
        var message = new
        {
            EventType = eventType,
            Product = product,
            Timestamp = DateTime.UtcNow
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        _channel.BasicPublish(exchange: "",
                             routingKey: "product_events",
                             basicProperties: null,
                             body: body);

        Console.WriteLine($"Published {eventType} event for product");
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}