using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProductService.Models;
using ProductService.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ProductService.Controllers;

[ApiController]
[Route("api/products")]
//[Authorize]
public class ProductsController(IProductServiceContract service, IDistributedCache cache) : ControllerBase
{
    private readonly IProductServiceContract _service = service;
    private readonly IDistributedCache _cache = cache;

    [HttpGet("health")]
    public async Task<IActionResult> Test()
    {
        return Ok("running...");
    }
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
        string cacheKey = $"product:{id}";
        // Check cache
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            var productFromCache = JsonSerializer.Deserialize<Product>(cached);
            return Ok(new { source = "cache", data = productFromCache });
        }

        // Simulate DB call
        var product = await _service.GetById(id);
        if (product == null)
            return NotFound();

        // Save to Redis
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(product), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
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
        await _cache.RemoveAsync($"product:{id}");
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
        await _cache.RemoveAsync($"product:{id}");
        return NoContent();
    }
}