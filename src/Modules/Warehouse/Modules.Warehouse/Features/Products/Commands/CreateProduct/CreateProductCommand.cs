using Common.SharedKernel.Domain.Entities;
using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Features.Categories.Domain;
using Modules.Warehouse.Features.Products.Domain;

namespace Modules.Warehouse.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, decimal Amount, string Sku, Guid CategoryId)
    : IRequest;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly WarehouseDbContext _dbContext;
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(WarehouseDbContext dbContext, IProductRepository productRepository, CancellationToken cancellationToken)
    {
        _dbContext = dbContext;
        _productRepository = productRepository;
    }

    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var money = new Money(Currency.Default, request.Amount);
        var sku = Sku.Create(request.Sku);
        ArgumentNullException.ThrowIfNull(sku);
        var categoryId = new CategoryId(request.CategoryId);
        var product = Product.Create(request.Name, money, sku, categoryId, _productRepository);

        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
