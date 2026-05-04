namespace Shared.Contracts;

public record OrderCreatedEvent
(
    Guid OrderId,
    Guid UserId,
    decimal TotalAmount,
    DateTime CreatedAt,
    Guid EventId,
    int Version = 1,
    string EventType = nameof(OrderCreatedEvent)
);

public record OrderCancelledEvent
(
    Guid OrderId,
    Guid UserId,
    decimal TotalAmount,
    DateTime CreatedAt,
    Guid EventId,
    int Version = 1,
    string EventType = nameof(OrderCancelledEvent)
);

public record PaymentSucceededEvent
(
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    DateTime PaidAt,
    Guid EventId,
    int Version = 1,
    string EventType = nameof(PaymentSucceededEvent)
);

public record PaymentFailedEvent
(
    Guid OrderId,
    DateTime FailedAt,
    Guid EventId,
    int Version = 1,
    string EventType = nameof(PaymentFailedEvent)
);

public record DeliveryCreatedEvent
(
    Guid TrackingId,
    Guid OrderId,
    Guid PaymentId,
    DateTime CreatedAt,
    Guid EventId,
    int Version = 1,
    string EventType = nameof(DeliveryCreatedEvent)
);

public record DeliveryShippedEvent
(
    Guid TrackingId,
    Guid OrderId,
    DateTime CreatedAt,
    Guid EventId,
    int Version = 1,
    string EventType = nameof(DeliveryShippedEvent)
);

public record DeliveryDeliveredEvent
(
    Guid TrackingId,
    Guid OrderId,
    DateTime CreatedAt,
    Guid EventId,
    int Version = 1,
    string EventType = nameof(DeliveryDeliveredEvent)
);


//public record DeliveryStartedEvent
//(
//    Guid OrderId,
//    Guid TrackingId,
//    DateTime ShippedAt,
//    int Version = 1
//);

public record ProductCreatedEvent
(
    Guid ProductId,
    string Name,
    decimal Price,
    int Stock,
    DateTime CreatedAt,
    Guid EventId,
    int Version = 1,
    string EventType = nameof(ProductCreatedEvent)
);

public record ProductUpdatedEvent
(
    Guid ProductId,
    string Name,
    decimal Price,
    int Stock,
    DateTime CreatedAt,
    Guid EventId,
    int Version = 1,
    string EventType = nameof(ProductUpdatedEvent)
);

public record ProductDeletedEvent
(
    Guid ProductId,
    string Name,
    decimal Price,
    int Stock,
    DateTime CreatedAt,
    Guid EventId,
    int Version = 1,
    string EventType = nameof(ProductDeletedEvent)
);

