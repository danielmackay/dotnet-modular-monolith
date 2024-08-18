using Common.SharedKernel.Domain.Base;
using Throw;

namespace Modules.Warehouse.Storage.Domain;

internal class Bay : Entity<int>
{
    private readonly List<Shelf> _shelves = [];

    public IReadOnlyList<Shelf> Shelves => _shelves.AsReadOnly();

    public int AvailableStorage => _shelves.Count(s => s.IsEmpty);

    public int TotalStorage => _shelves.Count;

    public string Name { get; private set; } = null!;

    public static Bay Create(int id, int numShelves)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numShelves);

        var bay = new Bay
        {
            Id = id,
            Name = $"Bay {id}"
        };

        for (var i = 1; i <= numShelves; i++)
        {
            var shelf = Shelf.Create(i);
            bay._shelves.Add(shelf);
        }

        return bay;
    }
}
