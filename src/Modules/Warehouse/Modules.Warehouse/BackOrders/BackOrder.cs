using Ardalis.SmartEnum;

namespace Modules.Warehouse.BackOrders;

internal record BackOrderId(Guid Value);

internal class BackOrder : AggregateRoot<BackOrderId>
{
    // private ProductId _productId = null!;

    // private int _quantityOrdered;

    // private int _quantityReceived;

    public BackOrderStatus Status { get; private set; } = null!;

    private BackOrder()
    {
    }

    public static BackOrder Create(/*ProductId productId, int quantityOrdered*/)
    {
        var backOrder = new BackOrder
        {
            Id = new BackOrderId(Uuid.Create()),
            // _productId = productId,
            // _quantityOrdered = quantityOrdered,
            // _quantityReceived = 0,
            Status = BackOrderStatus.Pending
        };

        return backOrder;
    }
}

internal class BackOrderStatus : SmartEnum<BackOrderStatus>
{
    public static readonly BackOrderStatus Pending = new(1, "Pending");
    public static readonly BackOrderStatus Received = new(2, "Received");

    private BackOrderStatus(int id, string name) : base(name, id)
    {
    }
}