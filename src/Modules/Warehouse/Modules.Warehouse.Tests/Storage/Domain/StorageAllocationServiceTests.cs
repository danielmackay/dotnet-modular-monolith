using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;

namespace Modules.Warehouse.Tests.Storage.Domain;

public class StorageAllocationServiceTests
{
    [Fact]
    public void AllocateStorage_ShouldAssignProductToFirstEmptyShelf()
    {
        // Arrange
        var productId = new ProductId();
        var aisles = new List<Aisle>
        {
            Aisle.Create("name", 2, 2)
        };

        // Act
        var result = StorageAllocationService.AllocateStorage(aisles, productId);

        // Assert
        result.IsError.Should().BeFalse();
        aisles[0].Bays[0].Shelves[0].ProductId.Should().Be(productId);
    }

    [Fact]
    public void AllocateStorage_ShouldThrowException_WhenNoEmptyShelfIsAvailable()
    {
        // Arrange
        var productId = new ProductId();
        var aisles = new List<Aisle>
        {
            Aisle.Create("name", 1, 1)
        };
        StorageAllocationService.AllocateStorage(aisles, productId);

        // Act
        var result = StorageAllocationService.AllocateStorage(aisles, productId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(StorageAllocationErrors.NoAvailableStorage);
    }


    [Fact]
    public void AllocateStorage_WhenMaxStorageUsed_ShouldHaveNoAvailableStorage()
    {
        // Arrange
        var numBays = 2;
        var numShelves = 3;
        var aisle = Aisle.Create("Aisle 1", numBays, numShelves);
        var productId = new ProductId();

        // Act
        for (var i = 0; i < numBays * numShelves; i++)
        {
            StorageAllocationService.AllocateStorage([aisle], productId);
        }

        // Assert
        aisle.TotalStorage.Should().Be(numBays * numShelves);
        aisle.AvailableStorage.Should().Be(0);
    }


    [Fact]
    public void AllocateStorage_ShouldThrowException_WhenNoEmptyShelf()
    {
        // Arrange
        var numBays = 2;
        var numShelves = 3;
        var aisle = Aisle.Create("Aisle 1", numBays, numShelves);
        var productId = new ProductId();

        // Act
        for (var i = 0; i < numBays * numShelves; i++)
        {
            StorageAllocationService.AllocateStorage([aisle], productId);
        }

        // Assert
        var result = StorageAllocationService.AllocateStorage([aisle], productId);
        result.IsError.Should().BeTrue();
    }
}