using ProductService.Models;

namespace ProductService.Services;

public interface IProductService
{
    Task<List<Product>> GetAll();
    Task<Product?> GetById(Guid id);
    Task<Product> Create(Product product);
    Task Update(Guid id, Product product);
    Task Delete(Guid id);
}