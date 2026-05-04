using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task ReduceStockAsync(int id, int quantity);
}

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;
    public ProductRepository(ProductDbContext context) => _context = context;

    public async Task<List<Product>> GetAllAsync() => await _context.Products.ToListAsync();

    public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task ReduceStockAsync(int id, int quantity)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            product.Stock -= quantity;
            await _context.SaveChangesAsync();
        }
    }
}
