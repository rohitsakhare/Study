using MongoDB.Driver;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<Product?> GetProductByIdAsync(string id);
    Task<Product?> GetProductBySKUAsync(string sku);
    Task<List<Product>> GetAllProductsAsync();
    Task<List<Product>> GetProductsByCategoryAsync(string category);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(string id, Product product);
    Task<bool> DeleteProductAsync(string id);
}
