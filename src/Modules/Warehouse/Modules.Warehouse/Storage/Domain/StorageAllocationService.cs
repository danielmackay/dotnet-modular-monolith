using ErrorOr;
using Modules.Warehouse.Products.Domain;

namespace Modules.Warehouse.Storage.Domain;

/// <summary>
/// Simple storage algorithm that naively assumes 1 product per shelf.
/// </summary>
internal class StorageAllocationService
{
    internal static ErrorOr<Shelf> AllocateStorage(IEnumerable<Aisle> aisles, ProductId productId)
    {
        foreach (var aisle in aisles)
        {
            if (aisle.AvailableStorage == 0)
                continue;

            return aisle.AssignProduct(productId);
        }

        return StorageAllocationErrors.NoAvailableStorage;
    }
}

public static class StorageAllocationErrors
{
    public static readonly Error NoAvailableStorage = Error.Failure("No available storage");
}