using Modules.Orders.Common;
using Modules.Orders.Orders.LineItem;
using Modules.Orders.Orders.Order;

namespace Modules.Orders.Tests.Orders;

public class LineItemTests
{
    [Fact]
    public void Create_ShouldInitializeLineItemWithCorrectValues()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = new Money(Currency.Default, 100);
        var quantity = 2;

        // Act
        var lineItem = LineItem.Create(orderId, productId, price, quantity);

        // Assert
        lineItem.OrderId.Should().Be(orderId);
        lineItem.ProductId.Should().Be(productId);
        lineItem.Price.Should().Be(price);
        lineItem.Quantity.Should().Be(quantity);
        lineItem.Id.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenPriceIsNegativeOrZero()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = new Money(Currency.Default, 0);
        var quantity = 2;
        Action act = () => LineItem.Create(orderId, productId, price, quantity);

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenQuantityIsNegativeOrZero()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = new Money(Currency.Default, 100);
        var quantity = 0;
        Action act = () => LineItem.Create(orderId, productId, price, quantity);

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddQuantity_ShouldIncreaseQuantity()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = new Money(Currency.Default, 100);
        var quantity = 2;
        var lineItem = LineItem.Create(orderId, productId, price, quantity);
        var additionalQuantity = 3;

        // Act
        lineItem.AddQuantity(additionalQuantity);

        // Assert
        lineItem.Quantity.Should().Be(quantity + additionalQuantity);
    }

    [Fact]
    public void RemoveQuantity_ShouldDecreaseQuantity()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = new Money(Currency.Default, 100);
        var quantity = 5;
        var lineItem = LineItem.Create(orderId, productId, price, quantity);
        var removeQuantity = 3;

        // Act
        lineItem.RemoveQuantity(removeQuantity);

        // Assert
        lineItem.Quantity.Should().Be(quantity - removeQuantity);
    }

    [Fact]
    public void RemoveQuantity_ShouldThrowException_WhenRemovingMoreThanAvailable()
    {
        // Arrange
        var orderId = new OrderId(Uuid.Create());
        var productId = new ProductId();
        var price = new Money(Currency.Default, 100);
        var quantity = 2;
        var lineItem = LineItem.Create(orderId, productId, price, quantity);
        var removeQuantity = 3;
        Action act = () => lineItem.RemoveQuantity(removeQuantity);

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }
}