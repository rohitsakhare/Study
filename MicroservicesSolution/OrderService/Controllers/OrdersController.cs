using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetOrders()
    {
        var orders = new[]
        {
            new { Id = 1, Product = "Laptop", User = "Alice" },
            new { Id = 2, Product = "Phone", User = "Bob" }
        };
        return Ok(orders);
    }
}