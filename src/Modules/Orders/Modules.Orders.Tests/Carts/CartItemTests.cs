using Modules.Orders.Carts.Domain;

namespace Modules.Orders.Tests.Carts;

public class CartItemTests
{
    [Fact]
    public void Create_ValidParameters_ShouldCreateCartItem()
    {
        // Arrange
        var productId = new ProductId();
        var quantity = 2;
        var unitPrice = Money.Create(100m);

        // Act
        var cartItem = CartItem.Create(productId, quantity, unitPrice);

        // Assert
        cartItem.ProductId.Should().Be(productId);
        cartItem.Quantity.Should().Be(quantity);
        cartItem.UnitPrice.Should().Be(unitPrice);
        cartItem.LinePrice.Amount.Should().Be(200m);
    }

    [Fact]
    public void Create_NegativeQuantity_ShouldThrow()
    {
        // Arrange
        var productId = new ProductId();
        var quantity = -1;
        var unitPrice = Money.Create(100m);

        // Act
        var act = () => CartItem.Create(productId, quantity, unitPrice);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_NegativeUnitPrice_ShouldThrow()
    {
        // Arrange
        var productId = new ProductId();
        var quantity = 2;
        var unitPrice = Money.Create(-100m);

        // Act
        var act = () => CartItem.Create(productId, quantity, unitPrice);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IncreaseQuantity_ShouldIncreaseQuantity()
    {
        // Arrange
        var productId = new ProductId();
        var quantity = 2;
        var unitPrice = Money.Create(100m);
        var cartItem = CartItem.Create(productId, quantity, unitPrice);

        // Act
        cartItem.IncreaseQuantity(3);

        // Assert
        cartItem.Quantity.Should().Be(5);
        cartItem.LinePrice.Amount.Should().Be(500m);
    }

    [Fact]
    public void DecreaseQuantity_ShouldDecreaseQuantity()
    {
        // Arrange
        var productId = new ProductId();
        var quantity = 5;
        var unitPrice = Money.Create(100m);
        var cartItem = CartItem.Create(productId, quantity, unitPrice);

        // Act
        cartItem.DecreaseQuantity(3);

        // Assert
        cartItem.Quantity.Should().Be(2);
        cartItem.LinePrice.Amount.Should().Be(200m);
    }

    [Fact]
    public void DecreaseQuantity_TooMany_ShouldThrow()
    {
        // Arrange
        var productId = new ProductId();
        var quantity = 2;
        var unitPrice = Money.Create(100m);
        var cartItem = CartItem.Create(productId, quantity, unitPrice);

        // Act
        var act = () => cartItem.DecreaseQuantity(3);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
