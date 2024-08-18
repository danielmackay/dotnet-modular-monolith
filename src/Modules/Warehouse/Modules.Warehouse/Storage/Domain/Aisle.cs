using Common.SharedKernel.Domain.Base;
using Throw;

namespace Modules.Warehouse.Storage.Domain;

internal record AisleId(Guid Value);

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
            Id = new AisleId(Guid.NewGuid()),
            Name = name
        };

        for (var i = 1; i <= numBays; i++)
        {
            var bay = Bay.Create(i, numShelves);
            aisle._bays.Add(bay);
        }

        return aisle;
    }
}
