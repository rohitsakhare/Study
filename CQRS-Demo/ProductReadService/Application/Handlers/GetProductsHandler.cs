
using Dapper;
using MediatR;
using ProductReadService.Application.Queries;
using ProductReadService.Domain.Entities;
using ProductReadService.Infrastructure.Data;

namespace ProductReadService.Application.Handlers;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly DbConnectionFactory _factory;

    public GetProductsHandler(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        using var conn = _factory.Create();

        return await conn.QueryAsync<ProductDto>(
            "SELECT Id, Name, Price FROM ProductView");
    }
}