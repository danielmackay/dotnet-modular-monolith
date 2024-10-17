using Modules.Warehouse.BackOrders.Domain;

namespace Modules.Warehouse.Tests.BackOrders;

public class BackOrderTests
{
    [Fact]
    public void Create_ProductId_ReturnsBackOrderWithCorrectProductId()
    {
        // Arrange
        var productId = new ProductId();

        // Act
        var backOrder = BackOrder.Create(productId);

        // Assert
        backOrder.ProductId.Should().Be(productId);
    }

    [Fact]
    public void Create_ProductId_ReturnsBackOrderWithDefaultQuantityOrdered()
    {
        // Arrange
        var productId = new ProductId();

        // Act
        var backOrder = BackOrder.Create(productId);

        // Assert
        backOrder.QuantityOrdered.Should().Be(10);
    }

    [Fact]
    public void Create_ProductId_ReturnsBackOrderWithZeroQuantityReceived()
    {
        // Arrange
        var productId = new ProductId();

        // Act
        var backOrder = BackOrder.Create(productId);

        // Assert
        backOrder.QuantityReceived.Should().Be(0);
    }

    [Fact]
    public void Create_ProductId_ReturnsBackOrderWithPendingStatus()
    {
        // Arrange
        var productId = new ProductId();

        // Act
        var backOrder = BackOrder.Create(productId);

        // Assert
        backOrder.Status.Should().Be(BackOrderStatus.Pending);
    }

    [Fact]
    public void Create_ProductId_ReturnsBackOrderWithNewId()
    {
        // Arrange
        var productId = new ProductId();

        // Act
        var backOrder = BackOrder.Create(productId);

        // Assert
        backOrder.Id.Should().NotBe(default(BackOrderId));
    }
}