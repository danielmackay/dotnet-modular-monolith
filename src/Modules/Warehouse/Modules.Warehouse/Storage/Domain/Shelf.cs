using Common.SharedKernel.Domain.Base;
using Modules.Warehouse.Products.Domain;
using Throw;

namespace Modules.Warehouse.Storage.Domain;

internal class Shelf : Entity<int>
{
    public string Name { get; private set; } = null!;

    public ProductId? ProductId { get; private set; }

    public bool IsEmpty => ProductId is null;

    public static Shelf Create(int number)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(number);

        return new Shelf
        {
            Id = number,
            Name = $"Shelf {number}"
        };
    }

    public void AssignProduct(ProductId productId)
    {
        ProductId = productId;
    }
}
