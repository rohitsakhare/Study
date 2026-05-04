using Shared.Contracts;
using Shared.Messaging.Abstractions;
using System.Text.Json;

namespace Shared.Outbox;

public interface IOutboxHandler
{
    string EventType { get; }
    Task Handle(string payload, IEventBus bus);
}

public class OrderCreatedHandler : IOutboxHandler
{
    public string EventType => nameof(OrderCreatedEvent);

    public Task Handle(string payload, IEventBus bus)
    {
        var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(payload)!;

        return bus.PublishAsync("order-events", evt);
    }
}

public class OrderCancelledHandler : IOutboxHandler
{
    public string EventType => nameof(OrderCancelledEvent);

    public Task Handle(string payload, IEventBus bus)
    {
        var evt = JsonSerializer.Deserialize<OrderCancelledEvent>(payload)!;

        return bus.PublishAsync("order-events", evt);
    }
}

public class PaymentSucceededHandler : IOutboxHandler
{
    public string EventType => nameof(PaymentSucceededEvent);

    public Task Handle(string payload, IEventBus bus)
    {
        var evt = JsonSerializer.Deserialize<PaymentSucceededEvent>(payload)!;

        return bus.PublishAsync("payment-events", evt);
    }
}

public class PaymentFailedHandler : IOutboxHandler
{
    public string EventType => nameof(PaymentFailedEvent);

    public Task Handle(string payload, IEventBus bus)
    {
        var evt = JsonSerializer.Deserialize<PaymentFailedEvent>(payload)!;

        return bus.PublishAsync("payment-events", evt);
    }
}

public class DeliveryCreatedHandler : IOutboxHandler
{
    public string EventType => nameof(DeliveryCreatedEvent);

    public Task Handle(string payload, IEventBus bus)
    {
        var evt = JsonSerializer.Deserialize<DeliveryCreatedEvent>(payload)!;

        return bus.PublishAsync("delivery-events", evt);
    }
}

public class DeliveryShippedHandler : IOutboxHandler
{
    public string EventType => nameof(DeliveryShippedEvent);

    public Task Handle(string payload, IEventBus bus)
    {
        var evt = JsonSerializer.Deserialize<DeliveryShippedEvent>(payload)!;

        return bus.PublishAsync("delivery-events", evt);
    }
}

public class DeliveryDeliveredHandler : IOutboxHandler
{
    public string EventType => nameof(DeliveryDeliveredEvent);

    public Task Handle(string payload, IEventBus bus)
    {
        var evt = JsonSerializer.Deserialize<DeliveryDeliveredEvent>(payload)!;

        return bus.PublishAsync("delivery-events", evt);
    }
}


public class ProductCreatedHandler : IOutboxHandler
{
    public string EventType => nameof(ProductCreatedEvent);

    public Task Handle(string payload, IEventBus bus)
    {
        var evt = JsonSerializer.Deserialize<ProductCreatedEvent>(payload)!;

        return bus.PublishAsync("product-events", evt);
    }
}

public class ProductUpdatedHandler : IOutboxHandler
{
    public string EventType => nameof(ProductUpdatedEvent);

    public Task Handle(string payload, IEventBus bus)
    {
        var evt = JsonSerializer.Deserialize<ProductUpdatedEvent>(payload)!;

        return bus.PublishAsync("product-events", evt);
    }
}

public class ProductDeletedHandler : IOutboxHandler
{
    public string EventType => nameof(ProductDeletedEvent);

    public Task Handle(string payload, IEventBus bus)
    {
        var evt = JsonSerializer.Deserialize<ProductDeletedEvent>(payload)!;

        return bus.PublishAsync("product-events", evt);
    }
}