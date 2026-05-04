using MongoDB.Driver;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _productsCollection;

    public ProductRepository(IMongoDbContext mongoDbContext)
    {
        _productsCollection = mongoDbContext.GetCollection<Product>("products");
    }

    public async Task<Product?> GetProductByIdAsync(string id)
    {
        return await _productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Product?> GetProductBySKUAsync(string sku)
    {
        return await _productsCollection.Find(p => p.SKU == sku).FirstOrDefaultAsync();
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _productsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _productsCollection.Find(p => p.Category == category).ToListAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        await _productsCollection.InsertOneAsync(product);
        return product;
    }

    public async Task<Product> UpdateProductAsync(string id, Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        await _productsCollection.ReplaceOneAsync(p => p.Id == id, product);
        return product;
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        var result = await _productsCollection.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }
}
