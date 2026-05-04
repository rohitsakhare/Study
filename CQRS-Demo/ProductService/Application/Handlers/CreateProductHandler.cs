using MediatR;
using ProductService.Application.Commands;
using ProductService.Domain;
using ProductService.Domain.Entities;
using ProductService.Domain.Outbox;
using ProductService.Infrastructure.Data;
using System.Text.Json;

namespace ProductService.Application.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly WriteDbContext _db;

    public CreateProductHandler(WriteDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price
        };

        _db.Products.Add(product);

        var evt = new ProductCreatedEvent
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };

        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(ProductCreatedEvent),
            Content = JsonSerializer.Serialize(evt),
            Processed = false
        });

        await _db.SaveChangesAsync();

        return product.Id;
    }
}