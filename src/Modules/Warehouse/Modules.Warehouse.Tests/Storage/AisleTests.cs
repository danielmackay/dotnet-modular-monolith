using FluentAssertions;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;
using Xunit.Abstractions;

namespace Modules.Warehouse.Tests;

public class AisleTests
{
    private readonly ITestOutputHelper _output;

    public AisleTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void LookingUpProduct_ReturnsAisleBayAndShelf()
    {
        // Exploratory
        var productA = new ProductId(Guid.NewGuid());
        var productB = new ProductId(Guid.NewGuid());
        var aisle = Aisle.Create("Aisle 1", 2, 3);
        var service = new StorageAllocationService();

        StorageAllocationService.AllocateStorage(new List<Aisle> { aisle }, productA);
        StorageAllocationService.AllocateStorage(new List<Aisle> { aisle }, productA);
        StorageAllocationService.AllocateStorage(new List<Aisle> { aisle }, productA);
        StorageAllocationService.AllocateStorage(new List<Aisle> { aisle }, productB);

        string aisleName = string.Empty;
        string bayName = string.Empty;
        string shelfName = string.Empty;

        // NOTE: If look ups like there were to cause performacne problems, two-way relationships could be added between
        // Different storage locations (e.g. Shelf->Bay->Aisle) to make lookups more efficient.
        foreach (var bay in aisle.Bays)
        {
            foreach (var shelf in bay.Shelves)
            {
                if (shelf.ProductId == productB)
                {
                    aisleName = aisle.Name;
                    bayName = bay.Name;
                    shelfName = shelf.Name;
                }
            }
        }

        _output.WriteLine($"Product B is in Aisle {aisleName}, Bay {bayName}, Shelf {shelfName}");
    }

    [Fact]
    public void Create_WithBaysAndShelves_CreatesCorrectNumberOfShelves()
    {
        // Arrange
        var numBays = 2;
        var numShelves = 3;

        // Act
        var sut = Aisle.Create("Aisle 1", numBays, numShelves);

        // Assert
        sut.TotalStorage.Should().Be(numBays * numShelves);
    }
}