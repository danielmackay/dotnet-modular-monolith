using Common.SharedKernel.Domain.Interfaces;
using Modules.Catalog.Categories.Domain;

namespace Modules.Catalog.Products.Domain;

internal record ProductId(Guid Value) : IStronglyTypedId<Guid>
{
    internal ProductId() : this(Uuid.Create())
    {
    }
}

internal class Product : AggregateRoot<ProductId>
{
    public string Name { get; private set; } = null!;

    public string Sku { get; private set; } = null!;

    public Money Price { get; private set; } = Money.Default;

    private readonly List<Category> _categories = [];

    public IReadOnlyList<Category> Categories => _categories.AsReadOnly();

    private Product()
    {
    }

    public static Product Create(string name, string sku, ProductId? id = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);

        var product = new Product
        {
            Name = name,
            Sku = sku,
            Id = id ?? new ProductId()
        };

        return product;
    }

    public void UpdatePrice(Money price)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price.Amount);
        Price = price;
    }

    public void AddCategory(Category category)
    {
        if (_categories.Contains(category))
            return;

        _categories.Add(category);
    }

    public void RemoveCategory(Category category)
    {
        if (!_categories.Contains(category))
            return;

        _categories.Remove(category);
    }
}