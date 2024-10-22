using ErrorOr;

namespace Modules.Warehouse.Products.Domain;

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
        ThrowIfNullOrWhiteSpace(name);

        var product = new Product
        {
            Id = new ProductId(),
            StockOnHand = 0
        };

        product.UpdateName(name);
        product.UpdateSku(sku);

        product.AddDomainEvent(new ProductCreatedEvent(product));

        return product;
    }

    private void UpdateName(string name)
    {
        ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    private void UpdateSku(Sku sku)
    {
        Sku = sku;
    }

    public ErrorOr<Success> RemoveStock(int quantity)
    {
        ThrowIfNegativeOrZero(quantity);

        if (StockOnHand - quantity < 0)
            return ProductErrors.CantRemoveMoreStockThanExists;

        StockOnHand -= quantity;

        if (StockOnHand <= LowStockThreshold)
            AddDomainEvent(new LowStockEvent(this));

        return Result.Success;
    }

    public void AddStock(int quantity)
    {
        ThrowIfNegativeOrZero(quantity);
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