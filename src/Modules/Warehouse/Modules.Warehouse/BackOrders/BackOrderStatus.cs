using Ardalis.SmartEnum;

namespace Modules.Warehouse.BackOrders;

internal class BackOrderStatus : SmartEnum<BackOrderStatus>
{
    public static readonly BackOrderStatus Pending = new(1, "Pending");
    public static readonly BackOrderStatus Received = new(2, "Received");

    private BackOrderStatus(int id, string name) : base(name, id)
    {
    }
}