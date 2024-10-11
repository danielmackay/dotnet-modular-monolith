using Common.SharedKernel.Domain.Ids;
using Modules.Orders.Orders.Domain.Order;

namespace Modules.Orders.Carts.Domain;

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