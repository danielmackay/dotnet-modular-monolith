using Common.SharedKernel.Domain.Ids;
using ErrorOr;
using Modules.Orders.Orders.Domain.Orders;

namespace Modules.Orders.Orders.Domain.LineItems;

internal class LineItem : Entity<LineItemId>
{
    private const decimal TaxRate = 0.1m;

    public required OrderId OrderId { get; init; }

    public required ProductId ProductId { get; init; }

    // Detach price from product to capture the price at the time of purchase
    public required Money Price { get; init; }

    public int Quantity { get; private set; }

    public Money Total => Price with { Amount = Price.Amount * Quantity };

    public Money Tax => Total * Total with { Amount = TaxRate };

    public Money TotalIncludingTax => Total + Tax;

    private LineItem() { }

    // NOTE: Need to use a factory, as EF does not let owned entities (i.e. Money) be passed via the constructor
    // Internal so that only the Order can create a LineItem
    internal static LineItem Create(OrderId orderId, ProductId productId, Money price, int quantity)
    {
        ThrowIfNegativeOrZero(price.Amount);
        ThrowIfNegativeOrZero(quantity);

        var lineItem = new LineItem()
        {
            Id = new LineItemId(Uuid.Create()),
            OrderId = orderId,
            ProductId = productId,
            Price = price,
            Quantity = quantity
        };

        return lineItem;
    }

    internal void AddQuantity(int quantity) => Quantity += quantity;

    internal ErrorOr<Success> RemoveQuantity(int quantity)
    {
        if (Quantity - quantity <= 0)
            return Error.Conflict("111", "Can't remove all units.  Remove the entire item instead");

        Quantity -= quantity;

        return new Success();
    }
}