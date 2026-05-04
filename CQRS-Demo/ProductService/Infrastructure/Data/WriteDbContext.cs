using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Outbox;

namespace ProductService.Infrastructure.Data;

public class WriteDbContext(DbContextOptions<WriteDbContext> opt) : DbContext(opt)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
}