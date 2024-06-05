using Ardalis.GuardClauses;
using Common.SharedKernel.Domain.Base;
using Common.SharedKernel.Domain.Entities;
using Common.SharedKernel.Domain.Exceptions;
using Common.SharedKernel.Domain.Identifiers;

namespace Module.Orders.Features.Orders;

internal class LineItem : Entity<LineItemId>
{
    public required OrderId OrderId { get; init; }

    public required ProductId ProductId { get; init; }

    //public Product? Product { get; init; }

    // Detatch price from product to capture the price at the time of purchase
    public required Money Price { get; init; }

    public int Quantity { get; private set; }

    public Money Total => new(Price.Currency, Price.Amount * Quantity);

    private LineItem() { }

    // NOTE: Need to use a factory, as EF does not let owned entities (i.e Money) be passed via the constructor
    // Internal so that only the Order can create a LineItem
    internal static LineItem Create(OrderId orderId, ProductId productId, Money price, int quantity)
    {
        Guard.Against.ZeroOrNegative(price.Amount);
        Guard.Against.ZeroOrNegative(quantity);

        var lineItem = new LineItem()
        {
            Id = new LineItemId(Guid.NewGuid()),
            OrderId = orderId,
            ProductId = productId,
            Price = price,
            Quantity = quantity
        };

        return lineItem;
    }

    internal void AddQuantity(int quantity) => Quantity += quantity;

    internal void RemoveQuantity(int quantity)
    {
        Guard.Against.Expression(_ => Quantity - quantity <= 0, quantity,
            "Can't remove all units.  Remove the entire item instead.");
        Quantity -= quantity;
    }
}
