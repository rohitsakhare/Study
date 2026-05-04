using InventoryService.Data;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.EventBus;
using Shared.Models;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly InventoryDbContext _context;
    private readonly KafkaProducer<InventoryReservedEvent> _producer;

    public InventoryController(InventoryDbContext context, IConfiguration config)
    {
        _context = context;
        var broker = config["Kafka:BootstrapServers"];
        var topic = config["Kafka:InventoryReservedTopic"];
        _producer = new KafkaProducer<InventoryReservedEvent>(broker, topic);

        // Start Kafka consumer for OrderCreated
        var orderTopic = config["Kafka:OrderCreatedTopic"];
        var consumer = new KafkaConsumer<OrderCreatedEvent>(broker, orderTopic, "inventory-service-group");
        consumer.StartConsuming(ProcessOrderCreatedEvent);
    }

    private async Task ProcessOrderCreatedEvent(OrderCreatedEvent orderEvent)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == orderEvent.ProductId);

        if (item != null && item.Quantity >= orderEvent.Quantity)
        {
            item.Quantity -= orderEvent.Quantity;
            await _context.SaveChangesAsync();

            var reservedEvent = new InventoryReservedEvent(orderEvent.OrderId, orderEvent.ProductId, orderEvent.Quantity);
            await _producer.PublishAsync(reservedEvent);
        }
        else
        {
            // TODO: publish InventoryOutOfStockEvent
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddStock([FromBody] InventoryItem item)
    {
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> GetInventory()
    {
        var items = await _context.InventoryItems.ToListAsync();
        return Ok(items);
    }
}
