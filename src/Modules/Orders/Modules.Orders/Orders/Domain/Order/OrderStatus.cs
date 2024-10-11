using Ardalis.SmartEnum;

namespace Modules.Orders.Orders.Order;

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
    public static readonly OrderStatus PaymentReceived = new(2, "PaymentReceived");
    public static readonly OrderStatus ReadyForShipping = new(3, "ReadyForShipping");
    public static readonly OrderStatus InTransit = new(4, "InTransit");
    public static readonly OrderStatus Delivered = new(5, "Delivered");

    private OrderStatus(int id, string name) : base(name, id)
    {
    }
}
