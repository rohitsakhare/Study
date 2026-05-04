using ProductService.Models;

namespace ProductService.Services;

public interface IProductServiceContract
{
    Task<List<Product>> GetAll();
    Task<Product?> GetById(Guid id);
    Task<Guid> Create(Product product);
    Task Update(Guid id, Product product);
    Task Delete(Guid id);
}