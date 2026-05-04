using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services;
using ProductService.Domain.Entities;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductServiceApp _service;
    public ProductController(ProductServiceApp service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllProductsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _service.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Add(Product product)
    {
        await _service.AddProductAsync(product);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }
}
