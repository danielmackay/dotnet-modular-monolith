using Modules.Orders.Orders.Domain.Orders;

namespace Modules.Orders.Tests.Orders;

internal class OrderBuilder
{
    private Order? _order;

    internal OrderBuilder NewOrder()
    {
        _order = Order.Create(new CustomerId(Uuid.Create()));
        return this;
    }

    internal OrderBuilder WithLineItem()
    {
        ArgumentNullException.ThrowIfNull(_order);
        var productId = new ProductId();
        var price = new Money(100);
        var quantity = 1;
        _order.AddLineItem(productId, price, quantity);
        return this;
    }

    internal OrderBuilder WithCashPayment()
    {
        ArgumentNullException.ThrowIfNull(_order);
        _order.AddCashPayment(_order.OrderTotal);
        return this;
    }

    internal Order Build()
    {
        ArgumentNullException.ThrowIfNull(_order);
        return _order;
    }
}