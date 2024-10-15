using Modules.Catalog.Products.Domain;

namespace Modules.Orders.Tests.Common.Builders;

internal class ProductBuilder
{
    private readonly Product _product = Product.Create("name", "12345678");

    internal ProductBuilder WithPrice()
    {
        var price = new Money(100);
        _product.UpdatePrice(price);
        return this;
    }

    internal Product Build() => _product;

}