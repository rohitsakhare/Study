using MediatR;

namespace ProductService.Application.Commands;

public record CreateProductCommand(string Name, decimal Price) : IRequest<Guid>;