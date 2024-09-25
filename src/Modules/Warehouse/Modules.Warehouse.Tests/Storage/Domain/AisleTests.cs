using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;
using Xunit.Abstractions;

namespace Modules.Warehouse.Tests.Storage.Domain;

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
        var productA = new ProductId();
        var productB = new ProductId();
        var aisle = Aisle.Create("Aisle 1", 2, 3);

        StorageAllocationService.AllocateStorage(new List<Aisle> { aisle }, productA);
        StorageAllocationService.AllocateStorage(new List<Aisle> { aisle }, productA);
        StorageAllocationService.AllocateStorage(new List<Aisle> { aisle }, productA);
        StorageAllocationService.AllocateStorage(new List<Aisle> { aisle }, productB);

        var aisleName = string.Empty;
        var bayName = string.Empty;
        var shelfName = string.Empty;

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

    [Fact]
    public void Create_WithBaysAndShelves_CreatesCorrectNumberOfBays()
    {
        // Arrange
        var numBays = 2;
        var numShelves = 3;

        // Act
        var sut = Aisle.Create("Aisle 1", numBays, numShelves);

        // Assert
        sut.Bays.Should().HaveCount(numBays);
    }

    [Fact]
    public void Create_WithBaysAndShelves_CreatesCorrectNumberOfShelvesPerBay()
    {
        // Arrange
        var numBays = 2;
        var numShelves = 3;

        // Act
        var sut = Aisle.Create("Aisle 1", numBays, numShelves);

        // Assert
        sut.Bays.Should().OnlyContain(bay => bay.Shelves.Count == numShelves);
    }

    [Fact]
    public void AssignProduct_WithAvailableStorage_AssignsProductToShelf()
    {
        // Arrange
        var productId = new ProductId();
        var sut = Aisle.Create("Aisle 1", 1, 1);

        // Act
        var result = sut.AssignProduct(productId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.ProductId.Should().Be(productId);
        var events = sut.PopDomainEvents();
        events.Should().ContainSingle();
        events[0].Should().BeOfType<ProductStoredEvent>();
    }

    [Fact]
    public void AssignProduct_WithNoAvailableStorage_ReturnsError()
    {
        // Arrange
        var productId = new ProductId();
        var sut = Aisle.Create("Aisle 1", 1, 1);
        sut.AssignProduct(productId);

        // Act
        var result = sut.AssignProduct(productId);

        // Assert
        result.IsError.Should().BeTrue();
    }
}