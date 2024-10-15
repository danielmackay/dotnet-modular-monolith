using Common.SharedKernel.Domain.Ids;
using Modules.Orders.Carts.Domain;

namespace Modules.Orders.Orders.Domain.Orders;

internal class OrderFactory
{
    internal static Order Checkout(Cart cart, CustomerId customerId)
    {
        var order = Order.Create(customerId);

        foreach (var item in cart.Items)
            order.AddLineItem(item.ProductId, item.UnitPrice, item.Quantity);

        return order;
    }
}