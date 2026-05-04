using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Services;

public class ProductServiceImplementation(AppDbContext context) : IProductServiceContract
{
    private readonly AppDbContext _context = context;

    public async Task<List<Product>> GetAll() => await _context.Products.AsNoTracking().ToListAsync();

    public async Task<Product?> GetById(Guid id) => await _context.Products.FindAsync(id);

    public async Task<Guid> Create(Product request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                Stock = request.Stock,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return product.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task Update(Guid id, Product request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var product = await _context.Products.FirstOrDefaultAsync(o => o.Id == id) ?? throw new Exception("Product not found");
            product.Name = request.Name;
            product.Price = request.Price;
            product.Stock = request.Stock;
            await _context.SaveChangesAsync();
            
            await transaction.CommitAsync();

        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task Delete(Guid id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var product = await _context.Products.FirstOrDefaultAsync(o => o.Id == id) ?? throw new Exception("Product not found");
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();                
                await transaction.CommitAsync();
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}