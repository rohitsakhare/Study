using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using Shared.Messaging;
using Shared.Outbox;

namespace ProductService.Infrastructure.Data;

public class AppDbContext : DbContext, IOutboxDbContext, IInboxDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<OutboxMessage>().ToTable("OutboxMessages");
        modelBuilder.Entity<InboxMessage>().ToTable("InboxMessages");
        modelBuilder.Entity<InboxMessage>().HasIndex(x => x.EventId).IsUnique();
    }
}