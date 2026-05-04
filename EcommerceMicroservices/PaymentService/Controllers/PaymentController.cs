using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;
using Shared.EventBus;
using Shared.Models;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly PaymentDbContext _context;
    private readonly KafkaProducer<PaymentSucceededEvent> _successProducer;
    private readonly KafkaProducer<PaymentFailedEvent> _failedProducer;

    public PaymentController(PaymentDbContext context, IConfiguration config)
    {
        _context = context;

        var broker = config["Kafka:BootstrapServers"];
        _successProducer = new KafkaProducer<PaymentSucceededEvent>(broker, config["Kafka:PaymentSucceededTopic"]);
        _failedProducer = new KafkaProducer<PaymentFailedEvent>(broker, config["Kafka:PaymentFailedTopic"]);

        // Start consuming OrderCreatedEvent
        var orderTopic = config["Kafka:OrderCreatedTopic"];
        var consumer = new KafkaConsumer<OrderCreatedEvent>(broker, orderTopic, "payment-service-group");
        consumer.StartConsuming(ProcessOrderCreatedEvent);
    }

    private async Task ProcessOrderCreatedEvent(OrderCreatedEvent orderEvent)
    {
        // Simulate payment processing
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderEvent.OrderId,
            Amount = orderEvent.TotalAmount
        };

        bool isSuccess = SimulatePayment();

        if (isSuccess)
        {
            payment.Status = "Succeeded";
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var successEvent = new PaymentSucceededEvent(orderEvent.OrderId, orderEvent.TotalAmount);
            await _successProducer.PublishAsync(successEvent);
        }
        else
        {
            payment.Status = "Failed";
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var failedEvent = new PaymentFailedEvent(orderEvent.OrderId, "Insufficient funds");
            await _failedProducer.PublishAsync(failedEvent);
        }
    }

    private bool SimulatePayment()
    {
        // Random success/failure for demo purposes
        return new Random().Next(0, 2) == 0;
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        var payments = await _context.Payments.ToListAsync();
        return Ok(payments);
    }
}