using ProductService.Models;

namespace ProductService.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAll();
    Task<Product?> GetById(Guid id);
    Task<Product> Create(Product product);
    Task Update(Product product);
    Task Delete(Guid id);
}