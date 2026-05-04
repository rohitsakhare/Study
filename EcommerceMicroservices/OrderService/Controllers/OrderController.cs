using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using OrderService.Models;
using Shared.EventBus;
using Shared.Models;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly KafkaProducer<OrderCreatedEvent> _producer;

    public OrderController(OrderDbContext context)
    {
        _context = context;
        _producer = new KafkaProducer<OrderCreatedEvent>("localhost:9092", "order-created");
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        order.Id = Guid.NewGuid();
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Publish event
        var evt = new OrderCreatedEvent(order.Id, order.ProductId, order.Quantity, order.TotalAmount);
        await _producer.PublishAsync(evt);

        return Ok(order);
    }
}
