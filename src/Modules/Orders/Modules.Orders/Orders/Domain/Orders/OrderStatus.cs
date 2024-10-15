using Ardalis.SmartEnum;

namespace Modules.Orders.Orders.Domain.Orders;

// internal enum OrderStatus
// {
//     None = 0,
//     PendingPayment = 1,
//     ReadyForShipping = 2,
//     InTransit = 3
// }

internal class OrderStatus : SmartEnum<OrderStatus>
{
    public static readonly OrderStatus New = new(1, "New");
    public static readonly OrderStatus Paid = new(2, "Paid");
    public static readonly OrderStatus Shipped = new(4, "Shipped");
    public static readonly OrderStatus Delivered = new(5, "Delivered");

    private OrderStatus(int id, string name) : base(name, id)
    {
    }
}