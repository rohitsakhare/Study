using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services;
using ProductService.Domain.Entities;

namespace ProductService.Controllers;

[ApiController]
[Route("api/products")]
//[Authorize]
public class ProductsController(IProductServiceContract service) : ControllerBase
{
    private readonly IProductServiceContract _service = service;

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _service.GetAll();
        return Ok(products);
    }

    // GET BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _service.GetById(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        var productId = await _service.Create(product);
        return Ok(new { ProductId = productId });
    }

    // UPDATE
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Product product)
    {
        var existing = await _service.GetById(id);
        if (existing == null)
            return NotFound();
        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        await _service.Update(id, existing);
        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _service.GetById(id);
        if (existing == null)
            return NotFound();
        await _service.Delete(id);
        return NoContent();
    }
}