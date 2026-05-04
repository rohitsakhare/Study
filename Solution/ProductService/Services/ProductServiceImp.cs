using ProductService.Models;
using ProductService.Repositories;

namespace ProductService.Services;

public class ProductServiceImp : IProductService
{
    private readonly IProductRepository _repo;

    public ProductServiceImp(IProductRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Product>> GetAll()
        => _repo.GetAll();

    public Task<Product?> GetById(Guid id)
        => _repo.GetById(id);

    public Task<Product> Create(Product product)
        => _repo.Create(product);

    public async Task Update(Guid id, Product product)
    {
        product.Id = id;
        await _repo.Update(product);
    }

    public Task Delete(Guid id)
        => _repo.Delete(id);
}