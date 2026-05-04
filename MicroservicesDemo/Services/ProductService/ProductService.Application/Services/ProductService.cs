using ProductService.Domain.Entities;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Application.Services;

public class ProductServiceApp
{
    private readonly IProductRepository _repo;
    public ProductServiceApp(IProductRepository repo) => _repo = repo;

    public Task<List<Product>> GetAllProductsAsync() => _repo.GetAllAsync();

    public Task<Product?> GetProductByIdAsync(int id) => _repo.GetByIdAsync(id);

    public Task AddProductAsync(Product product) => _repo.AddAsync(product);

    public Task ReduceStockAsync(int id, int quantity) => _repo.ReduceStockAsync(id, quantity);
}
