using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Warehouse.Storage.Domain;

internal record BayId(Guid Value) : IStronglyTypedId<Guid>
{
    internal BayId() : this(Uuid.Create())
    {
    }
}

internal class Bay : Entity<BayId>
{
    private readonly List<Shelf> _shelves = [];

    public IReadOnlyList<Shelf> Shelves => _shelves.AsReadOnly();

    public int AvailableStorage => _shelves.Count(s => s.IsEmpty);

    public int TotalStorage => _shelves.Count;

    public string Name { get; private set; } = null!;

    public static Bay Create(string name, int numShelves)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numShelves);

        var bay = new Bay
        {
            Id = new BayId(),
            Name = name
        };

        for (var i = 1; i <= numShelves; i++)
        {
            var shelf = Shelf.Create($"Shelf {i}");
            bay._shelves.Add(shelf);
        }

        return bay;
    }
}