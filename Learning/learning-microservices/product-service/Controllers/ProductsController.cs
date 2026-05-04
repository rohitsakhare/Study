using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using product_service.Data;
using product_service.Models;
using product_service.Services;

namespace product_service.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductDbContext _db;
    private readonly RabbitMqService _rabbitMq;

    public ProductsController(ProductDbContext db, RabbitMqService rabbitMq)
    {
        _db = db;
        _rabbitMq = rabbitMq;
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll()
    {
        return await _db.Products
            .AsNoTracking()
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Product>> GetById(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        product.Id = Guid.NewGuid();

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // Publish product created event
        _rabbitMq.PublishProductEvent("ProductCreated", product);

        return CreatedAtAction(nameof(GetById),
            new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Product updated)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        product.Name = updated.Name;
        product.Price = updated.Price;

        await _db.SaveChangesAsync();

        // Publish product updated event
        _rabbitMq.PublishProductEvent("ProductUpdated", product);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        // Publish product deleted event
        _rabbitMq.PublishProductEvent("ProductDeleted", product);

        return NoContent();
    }
}
