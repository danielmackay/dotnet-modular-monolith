using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.Common.Persistence;

namespace Modules.Warehouse.Features.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<IEnumerable<ProductDto>>;

public record ProductDto(Guid Id, string Sku, string Name, decimal Price);

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly WarehouseDbContext _dbContext;

    public GetProductsQueryHandler(WarehouseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .Select(p => new ProductDto(p.Id.Value, p.Sku.Value, p.Name, p.Price.Amount))
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
