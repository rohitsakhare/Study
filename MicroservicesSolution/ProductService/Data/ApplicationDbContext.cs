using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Lowercase table & column names for PostgreSQL
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()!.ToLower());
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName()!.ToLower());
            }
        }

        // Optional: Configure default values or constraints if needed
        // Example: IsActive default true
        modelBuilder.Entity<Product>()
            .Property(p => p.IsActive)
            .HasDefaultValue(true);
    }

    /// <summary>
    /// Dynamically seeds initial products with current UTC date/time.
    /// Call this after applying migrations in Program.cs
    /// </summary>
    public void SeedInitialProducts()
    {
        if (!Products.Any())
        {
            var now = DateTime.UtcNow;

            Products.AddRange(
                new Product
                {
                    Name = "Laptop",
                    Description = "High-performance laptop for professionals",
                    Price = 1299.99m,
                    StockQuantity = 50,
                    Category = "Electronics",
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Name = "Phone",
                    Description = "Latest smartphone with advanced features",
                    Price = 899.99m,
                    StockQuantity = 100,
                    Category = "Electronics",
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Name = "Headphones",
                    Description = "Wireless noise-cancelling headphones",
                    Price = 199.99m,
                    StockQuantity = 75,
                    Category = "Electronics",
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                }
            );

            SaveChanges();
        }
    }
}