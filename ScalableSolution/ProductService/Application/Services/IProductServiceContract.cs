using ProductService.Domain.Entities;

namespace ProductService.Application.Services;

public interface IProductServiceContract
{
    Task<List<Product>> GetAll();
    Task<Product?> GetById(Guid id);
    Task<Guid> Create(Product product);
    Task Update(Guid id, Product product);
    Task Delete(Guid id);
}