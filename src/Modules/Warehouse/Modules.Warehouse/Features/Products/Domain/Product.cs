using Ardalis.GuardClauses;
using Common.SharedKernel.Domain.Base;
using Common.SharedKernel.Domain.Entities;
using Common.SharedKernel.Domain.Exceptions;
using Common.SharedKernel.Domain.Identifiers;
using Modules.Warehouse.Features.Categories.Domain;

namespace Modules.Warehouse.Features.Products.Domain;

internal class Product : AggregateRoot<ProductId>
{
    private const int LowStockThreshold = 5;

    public CategoryId CategoryId { get; set; } = null!;

    public Category Category { get; set; } = null!;

    public string Name { get; private set; } = null!;

    public Money Price { get; private set; } = null!;

    public Sku Sku { get; private set; } = null!;

    public int StockOnHand { get; private set; }

    private Product()
    {
    }

    // NOTE: Need to use a factory, as EF does not let owned entities (i.e Money & Sku) be passed via the constructor
    public static Product Create(string name, Money price, Sku sku, CategoryId categoryId, IProductRepository productRepository)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.Null(sku);
        Guard.Against.Null(price);
        Guard.Against.ZeroOrNegative(price.Amount);
        Guard.Against.Null(categoryId);

        var product = new Product
        {
            Id = new ProductId(Guid.NewGuid()),
            CategoryId = categoryId,
            Name = name,
            Price = price,
            StockOnHand = 0
        };

        product.UpdateSku(sku, productRepository);

        product.AddDomainEvent(ProductCreatedEvent.Create(product));

        return product;
    }

    public void UpdateName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Name = name;
    }

    public void UpdatePrice(Money price)
    {
        Guard.Against.Null(price);
        Guard.Against.ZeroOrNegative(price.Amount);
        Price = price;
    }

    public void UpdateSku(Sku sku, IProductRepository productRepository)
    {
        Guard.Against.Null(sku);

        if (productRepository.SkuExists(sku))
            throw new ArgumentException("Sku already exists");

        Sku = sku;
    }

    public void RemoveStock(int quantity)
    {
        Guard.Against.NegativeOrZero(quantity);

        if (StockOnHand - quantity < 0)
            throw new DomainException("Cannot adjust stock below zero");

        StockOnHand -= quantity;

        if (StockOnHand <= LowStockThreshold)
            AddDomainEvent(new LowStockEvent(Id));
    }

    public void AddStock(int quantity)
    {
        Guard.Against.NegativeOrZero(quantity);
        StockOnHand += quantity;
    }
}
