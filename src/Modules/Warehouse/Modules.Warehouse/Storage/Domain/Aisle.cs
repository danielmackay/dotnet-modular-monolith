using Common.SharedKernel.Domain.Interfaces;
using ErrorOr;
using Modules.Warehouse.Products.Domain;

namespace Modules.Warehouse.Storage.Domain;

internal record AisleId(Guid Value) : IStronglyTypedId<Guid>
{
    internal AisleId() : this(Uuid.Create())
    {
    }
}

internal class Aisle : AggregateRoot<AisleId>
{
    public string Name { get; private set; } = null!;

    public int TotalStorage => _bays.Sum(b => b.TotalStorage);

    public int AvailableStorage => _bays.Sum(b => b.AvailableStorage);

    private readonly List<Bay> _bays = [];

    public IReadOnlyList<Bay> Bays => _bays.AsReadOnly();

    private Aisle() { }

    public static Aisle Create(string name, int numBays, int numShelves)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numBays);

        var aisle = new Aisle
        {
            Id = new AisleId(),
            Name = name
        };

        for (var i = 1; i <= numBays; i++)
        {
            var bay = Bay.Create($"Bay {i}", numShelves);
            aisle._bays.Add(bay);
        }

        return aisle;
    }

    public ErrorOr<Shelf> AssignProduct(ProductId productId)
    {
        ArgumentNullException.ThrowIfNull(productId);

        var shelf = _bays
            .SelectMany(b => b.Shelves)
            .FirstOrDefault(s => s.IsEmpty);

        if (AvailableStorage == 0 || shelf is null)
            return StorageAllocationErrors.NoAvailableStorage;

        shelf.AssignProduct(productId);

        AddDomainEvent(new ProductStoredEvent(productId));

        return shelf;
    }
}