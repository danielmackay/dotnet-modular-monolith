using Common.SharedKernel.Domain.Entities;
using Modules.Warehouse.Common.Interfaces;
using Modules.Warehouse.Domain.Categories;
using Modules.Warehouse.Domain.Products;

namespace Modules.Warehouse.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, decimal Amount, string Sku, Guid CategoryId)
    : IRequest;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly IWarehouseDbContext _dbContext;
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IWarehouseDbContext dbContext, IProductRepository productRepository, CancellationToken cancellationToken)
    {
        _dbContext = dbContext;
        _productRepository = productRepository;
    }

    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var money = new Money(Currency.Default, request.Amount);
        var sku = Sku.Create(request.Sku);
        var categoryId = new CategoryId(request.CategoryId);
        var product = Product.Create(request.Name, money, sku, categoryId, _productRepository);

        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
