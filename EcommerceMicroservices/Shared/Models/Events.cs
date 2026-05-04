namespace Shared.Models;

public record OrderCreatedEvent(Guid OrderId, Guid ProductId, int Quantity, decimal TotalAmount);
public record InventoryReservedEvent(Guid OrderId, Guid ProductId, int Quantity);
public record PaymentSucceededEvent(Guid OrderId, decimal Amount);
public record PaymentFailedEvent(Guid OrderId, string Reason);
