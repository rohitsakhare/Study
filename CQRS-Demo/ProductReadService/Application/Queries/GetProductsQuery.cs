using MediatR;
using ProductReadService.Domain.Entities;

namespace ProductReadService.Application.Queries;

public record GetProductsQuery() : IRequest<IEnumerable<ProductDto>>;