using Modules.Orders.Common;

namespace Modules.Orders.Tests.Cart;

public class CartTests
{
    [Fact]
    public void AddItem_ShouldIncreaseQuantity_WhenItemAlreadyExists()
    {
        // Arrange
        var productId = new ProductId();
        var unitPrice = new Money(Currency.Default, 10);
        var cart = Carts.Cart.Create(productId, 1, unitPrice);

        // Act
        cart.AddItem(productId, 2, unitPrice);

        // Assert
        var item = cart.Items.First(i => i.ProductId == productId);
        item.Quantity.Should().Be(3);
        item.LinePrice.Amount.Should().Be(30);
    }

    [Fact]
    public void AddItem_ShouldAddNewItem_WhenItemDoesNotExist()
    {
        // Arrange
        var productId1 = new ProductId();
        var productId2 = new ProductId();
        var unitPrice = new Money(Currency.Default, 10);
        var cart = Carts.Cart.Create(productId1, 1, unitPrice);

        // Act
        cart.AddItem(productId2, 2, unitPrice);

        // Assert
        cart.Items.Count.Should().Be(2);
        var item = cart.Items.First(i => i.ProductId == productId2);
        item.Quantity.Should().Be(2);
        item.LinePrice.Amount.Should().Be(20);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItem_WhenItemExists()
    {
        // Arrange
        var productId = new ProductId();
        var unitPrice = new Money(Currency.Default, 10);
        var cart = Carts.Cart.Create(productId, 1, unitPrice);

        // Act
        cart.RemoveItem(productId);

        // Assert
        cart.Items.Should().BeEmpty();
        cart.TotalPrice.Amount.Should().Be(0);
    }

    [Fact]
    public void RemoveItem_ShouldDoNothing_WhenItemDoesNotExist()
    {
        // Arrange
        var productId1 = new ProductId();
        var productId2 = new ProductId();
        var unitPrice = new Money(Currency.Default, 10);
        var cart = Carts.Cart.Create(productId1, 1, unitPrice);

        // Act
        cart.RemoveItem(productId2);

        // Assert
        cart.Items.Should().HaveCount(1);
        cart.TotalPrice.Amount.Should().Be(10);
    }

    [Fact]
    public void UpdateTotal_ShouldCalculateTotalPriceCorrectly()
    {
        // Arrange
        var productId1 = new ProductId();
        var productId2 = new ProductId();
        var unitPrice1 = new Money(Currency.Default, 10);
        var unitPrice2 = new Money(Currency.Default, 20);
        var cart = Carts.Cart.Create(productId1, 1, unitPrice1);

        // Act
        cart.AddItem(productId2, 2, unitPrice2);

        // Assert
        cart.TotalPrice.Amount.Should().Be(50);
    }
}