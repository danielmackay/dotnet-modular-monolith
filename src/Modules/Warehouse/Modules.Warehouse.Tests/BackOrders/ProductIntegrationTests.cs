using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.BackOrders.Domain;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Tests.Common;
using Modules.Warehouse.Tests.Common.Builders;
using Xunit.Abstractions;

namespace Modules.Warehouse.Tests.BackOrders;

public class BackOrderIntegrationTests(WarehouseDatabaseFixture fixture, ITestOutputHelper output)
    : WarehouseIntegrationTestBase(fixture, output)
{
    [Fact]
    public async Task LowStockEvent_CreatesBackOrder()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        await Database.AddEntityAsync(product);
        var evt = new LowStockEvent(product);

        // Act
        await Mediator.Publish(evt);

        // Assert
        var backOrders = await Database.GetQueryable<BackOrder>().ToListAsync();
        backOrders.Should().HaveCount(1);

        var backOrder = backOrders.First();
        backOrder.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        backOrder.CreatedBy.Should().NotBeNullOrWhiteSpace();
        backOrder.ProductId.Should().Be(evt.Product.Id);
        backOrder.OrderReference.Should().NotBeNullOrWhiteSpace();
        backOrder.QuantityOrdered.Should().Be(10);
        backOrder.QuantityReceived.Should().Be(0);
    }

    [Fact]
    public async Task LowStockEvent_WithExistingBackOrder_DoesNotCreateNewBackOrder()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        await Database.AddEntityAsync(product);
        var existingBackOrder = new BackOrderBuilder().WithProduct(product.Id).Build();
        await Database.AddEntityAsync(existingBackOrder);
        var evt = new LowStockEvent(product);

        // Act
        await Mediator.Publish(evt);

        // Assert
        var backOrders = await Database.GetQueryable<BackOrder>().ToListAsync();
        backOrders.Should().HaveCount(1);

        var backOrder = backOrders.First();
        backOrder.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        backOrder.CreatedBy.Should().NotBeNullOrWhiteSpace();
        backOrder.ProductId.Should().Be(evt.Product.Id);
        backOrder.OrderReference.Should().NotBeNullOrWhiteSpace();
        backOrder.QuantityOrdered.Should().Be(10);
        backOrder.QuantityReceived.Should().Be(0);
    }
}