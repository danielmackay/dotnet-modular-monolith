using Modules.Warehouse.Products.Domain;

namespace Modules.Warehouse.Tests.Products;

public class ProductTests
{
    [Fact]
    public void Create_ShouldInitializeProductCorrectly()
    {
        // Arrange
        var name = "Test Product";
        var sku = Sku.Create("12345678");

        // Act
        var product = Product.Create(name, sku);

        // Assert
        product.Name.Should().Be(name);
        product.Sku.Should().Be(sku);
        product.StockOnHand.Should().Be(0);
        product.Id.Should().NotBeNull();
    }

    [Fact]
    public void RemoveStock_ShouldDecreaseStockOnHand()
    {
        // Arrange
        var product = Product.Create("Test Product", Sku.Create("12345678"));
        product.AddStock(10);
        var quantityToRemove = 5;

        // Act
        product.RemoveStock(quantityToRemove);

        // Assert
        product.StockOnHand.Should().Be(5);
    }

    [Fact]
    public void RemoveStock_ShouldThrowException_WhenStockGoesBelowZero()
    {
        // Arrange
        var product = Product.Create("Test Product", Sku.Create("12345678"));
        product.AddStock(5);
        var quantityToRemove = 10;

        // Act
        var result = product.RemoveStock(quantityToRemove);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProductErrors.CantRemoveMoreStockThanExists);
    }

    [Fact]
    public void AddStock_ShouldIncreaseStockOnHand()
    {
        // Arrange
        var product = Product.Create("Test Product", Sku.Create("12345678"));
        var quantityToAdd = 5;

        // Act
        product.AddStock(quantityToAdd);

        // Assert
        product.StockOnHand.Should().Be(5);
    }
}
