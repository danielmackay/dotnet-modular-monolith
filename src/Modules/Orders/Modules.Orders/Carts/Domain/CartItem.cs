using Common.SharedKernel.Domain.Ids;
using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Orders.Carts.Domain;

internal record CartItemId(Guid Value) : IStronglyTypedId<Guid>
{
    public CartItemId() : this(Uuid.Create())
    {
    }
}

internal class CartItem : Entity<CartItemId>
{
    public ProductId ProductId { get; private set; } = null!;

    public int Quantity { get; private set; }

    public Money UnitPrice { get; private set; } = null!;

    public Money LinePrice { get; private set; } = null!;

    public static CartItem Create(ProductId productId, int quantity, Money unitPrice)
    {
        ArgumentNullException.ThrowIfNull(productId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitPrice.Amount);

        var cartItem = new CartItem
        {
            Id = new CartItemId(),
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
        };

        cartItem.UpdateLinePrice();

        return cartItem;
    }

    public void IncreaseQuantity(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        Quantity += quantity;
        UpdateLinePrice();
    }

    public void DecreaseQuantity(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        if (Quantity - quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Cannot remove more items than the cart contains.");

        Quantity -= quantity;
        UpdateLinePrice();
    }

    private void UpdateLinePrice() => LinePrice = UnitPrice with { Amount = UnitPrice.Amount * Quantity };

    // Needed for EF
    private CartItem()
    {
    }
}
