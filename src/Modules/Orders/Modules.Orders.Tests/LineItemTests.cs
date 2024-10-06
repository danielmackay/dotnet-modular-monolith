using Modules.Orders.Orders.LineItem;
using Modules.Orders.Orders.Order;

namespace Modules.Orders.Tests;

public class LineItemTests
{
    [Fact]
    public void Create_ValidParameters_ShouldCreateLineItem()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = Money.Create(100m);
        var quantity = 2;

        // Act
        var lineItem = LineItem.Create(orderId, productId, price, quantity);

        // Assert
        lineItem.OrderId.Should().Be(orderId);
        lineItem.ProductId.Should().Be(productId);
        lineItem.Price.Should().Be(price);
        lineItem.Quantity.Should().Be(quantity);
    }

    [Fact]
    public void Create_NegativePrice_ShouldThrow()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = Money.Create(-100m);
        var quantity = 2;

        // Act
        var act = () => LineItem.Create(orderId, productId, price, quantity);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_ZeroQuantity_ShouldThrow()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = Money.Create(100m);
        var quantity = 0;

        // Act
        var act = () => LineItem.Create(orderId, productId, price, quantity);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Total_ShouldReturnCorrectAmount()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = Money.Create(100m);
        var quantity = 2;

        // Act
        var lineItem = LineItem.Create(orderId, productId, price, quantity);

        // Assert
        lineItem.Total.Amount.Should().Be(200m);
    }

    [Fact]
    public void Tax_ShouldReturnCorrectAmount()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = Money.Create(100m);
        var quantity = 2;

        // Act
        var lineItem = LineItem.Create(orderId, productId, price, quantity);

        // Assert
        lineItem.Tax.Amount.Should().Be(20m);
    }

    [Fact]
    public void TotalIncludingTax_ShouldReturnCorrectAmount()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = Money.Create(100m);
        var quantity = 2;

        // Act
        var lineItem = LineItem.Create(orderId, productId, price, quantity);

        // Assert
        lineItem.TotalIncludingTax.Amount.Should().Be(220m);
    }

    [Fact]
    public void AddQuantity_ShouldIncreaseQuantity()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = Money.Create(100m);
        var quantity = 2;
        var lineItem = LineItem.Create(orderId, productId, price, quantity);

        // Act
        lineItem.AddQuantity(3);

        // Assert
        lineItem.Quantity.Should().Be(5);
    }

    [Fact]
    public void RemoveQuantity_ShouldDecreaseQuantity()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = Money.Create(100m);
        var quantity = 5;
        var lineItem = LineItem.Create(orderId, productId, price, quantity);

        // Act
        lineItem.RemoveQuantity(3);

        // Assert
        lineItem.Quantity.Should().Be(2);
    }

    [Fact]
    public void RemoveQuantity_TooMany_ShouldThrow()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = Money.Create(100m);
        var quantity = 2;
        var lineItem = LineItem.Create(orderId, productId, price, quantity);

        // Act
        var act = () => lineItem.RemoveQuantity(3);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}