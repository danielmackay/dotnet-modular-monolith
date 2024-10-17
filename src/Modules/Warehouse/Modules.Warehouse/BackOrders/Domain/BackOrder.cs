using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Warehouse.BackOrders.Domain;

internal record BackOrderId(Guid Value) : IStronglyTypedId<Guid>
{
    internal BackOrderId() : this(Uuid.Create())
    {
    }
}

internal class BackOrder : AggregateRoot<BackOrderId>
{
    private const int DefaultQuantityToOrder = 10;

    public ProductId ProductId { get; private set; } = null!;

    public int QuantityOrdered { get; private set; }

    public int QuantityReceived { get; private set; }

    public string? OrderReference { get; private set; }

    public BackOrderStatus Status { get; private set; } = null!;

    // Needed for EF
    private BackOrder()
    {
    }

    public static BackOrder Create(ProductId productId)
    {
        var backOrder = new BackOrder
        {
            Id = new BackOrderId(Uuid.Create()),
            ProductId = productId,
            QuantityOrdered = DefaultQuantityToOrder,
            QuantityReceived = 0,
            Status = BackOrderStatus.Pending
        };

        return backOrder;
    }

    public void UpdateReference(string reference)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);
        OrderReference = reference;
    }

    public void UpdateReceived(int quantity)
    {
        QuantityReceived = quantity;

        if (QuantityReceived >= QuantityOrdered)
            Status = BackOrderStatus.Received;
    }
}