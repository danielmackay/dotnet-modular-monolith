using Modules.Warehouse.BackOrders.Domain;
using Modules.Warehouse.Products.Domain;

namespace Modules.Warehouse.Tests.Common.Builders;

internal class ProductBuilder
{
    private readonly Product _product = Product.Create("product name", Sku.Create("12345678"));

    internal Product Build() => _product;
}

internal class BackOrderBuilder
{
    private BackOrder _backOrder = Create();

    internal BackOrderBuilder WithProduct(ProductId productId)
    {
        _backOrder = Create(productId);
        return this;
    }

    internal BackOrder Build() => _backOrder;

    private static BackOrder Create(ProductId? productId = null)
    {
        productId = productId ?? new ProductId();
        var backOrder = BackOrder.Create(productId);
        backOrder.UpdateReference("Reference");
        return backOrder;
    }
}