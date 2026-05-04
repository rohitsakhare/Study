using Microsoft.AspNetCore.Mvc;
using Shared.EventBus;
using Shared.Models;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    public NotificationController(IConfiguration config)
    {
        var broker = config["Kafka:BootstrapServers"];

        // Consume Payment Succeeded
        var paymentSuccessConsumer = new KafkaConsumer<PaymentSucceededEvent>(
            broker, config["Kafka:PaymentSucceededTopic"], "notification-service-group");
        paymentSuccessConsumer.StartConsuming(HandlePaymentSucceeded);

        // Consume Payment Failed
        var paymentFailedConsumer = new KafkaConsumer<PaymentFailedEvent>(
            broker, config["Kafka:PaymentFailedTopic"], "notification-service-group");
        paymentFailedConsumer.StartConsuming(HandlePaymentFailed);

        // Consume Inventory Reserved or Order Confirmed if needed
        // Example:
        // var inventoryReservedConsumer = new KafkaConsumer<InventoryReservedEvent>(
        //     broker, "inventory-reserved", "notification-service-group");
        // inventoryReservedConsumer.StartConsuming(HandleInventoryReserved);
    }

    private Task HandlePaymentSucceeded(PaymentSucceededEvent evt)
    {
        Console.WriteLine($"[Notification] Payment succeeded for Order {evt.OrderId}, Amount: {evt.Amount}");
        // Here you can call an Email/SMS service
        return Task.CompletedTask;
    }

    private Task HandlePaymentFailed(PaymentFailedEvent evt)
    {
        Console.WriteLine($"[Notification] Payment FAILED for Order {evt.OrderId}, Reason: {evt.Reason}");
        // Here you can call an Email/SMS service
        return Task.CompletedTask;
    }

    // Optional: Inventory notifications
    // private Task HandleInventoryReserved(InventoryReservedEvent evt) { ... }
}
