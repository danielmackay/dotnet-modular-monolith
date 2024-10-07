using ErrorOr;
using Throw;

namespace Modules.Warehouse.Products.Domain;

// internal record ProductId(Guid Value) : IStronglyTypedId<Guid>
// {
//     internal ProductId() : this(Uuid.Create())
//     {
//     }
// }

internal class Product : AggregateRoot<ProductId>
{
    private const int LowStockThreshold = 5;

    public string Name { get; private set; } = null!;

    public Sku Sku { get; private set; } = null!;

    public int StockOnHand { get; private set; }

    private Product()
    {
    }

    // NOTE: Need to use a factory, as EF does not let owned entities (i.e. Money & Sku) be passed via the constructor
    public static Product Create(string name, Sku sku)
    {
        // TODO: Check for SKU uniqueness in Application
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var product = new Product
        {
            Id = new ProductId(),
            StockOnHand = 0
        };

        product.UpdateName(name);
        product.UpdateSku(sku);

        product.AddDomainEvent(ProductCreatedEvent.Create(product));

        return product;
    }

    private void UpdateName(string name)
    {
        name.Throw().IfEmpty();
        Name = name;
    }

    private void UpdateSku(Sku sku)
    {
        Sku = sku;
    }

    public ErrorOr<Success> RemoveStock(int quantity)
    {
        quantity.Throw().IfNegativeOrZero();

        if (StockOnHand - quantity < 0)
            return ProductErrors.CantRemoveMoreStockThanExists;

        StockOnHand -= quantity;

        if (StockOnHand <= LowStockThreshold)
            AddDomainEvent(new LowStockEvent(Id));

        return Result.Success;
    }

    public void AddStock(int quantity)
    {
        quantity.Throw().IfNegativeOrZero();
        StockOnHand += quantity;
    }
}

// internal class GetAllProductsSpecification : Specification<Product>
// {
//     public GetAllProductsSpecification()
//     {
//
//     }
// }
