using Modules.Orders.Orders.Domain.Orders;

namespace Modules.Orders.Tests.Common.Builders;

internal class OrderBuilder
{
    private readonly Order _order = Order.Create(new CustomerId(Uuid.Create()));

    internal OrderBuilder WithLineItem()
    {
        var productId = new ProductId();
        var price = new Money(100);
        var quantity = 1;
        _order.AddLineItem(productId, price, quantity);
        return this;
    }

    internal OrderBuilder WithCashPayment()
    {
        _order.AddCashPayment(_order.OrderTotal);
        return this;
    }

    internal Order Build() => _order;
}