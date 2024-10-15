using Common.SharedKernel.Domain.Interfaces;
using ErrorOr;
using Modules.Warehouse.Products.Domain;
using Success = OneOf.Types.Success;

namespace Modules.Warehouse.Storage.Domain;

internal record ShelfId(Guid Value) : IStronglyTypedId<Guid>
{
    internal ShelfId() : this(Uuid.Create())
    {
    }
}

internal class Shelf : AggregateRoot<ShelfId>
{
    public string Name { get; private set; } = null!;

    public ProductId? ProductId { get; private set; }

    public bool IsEmpty => ProductId is null;

    public static Shelf Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Shelf
        {
            Id = new ShelfId(),
            Name = name
        };
    }

    public void AssignProduct(ProductId productId)
    {
        ArgumentNullException.ThrowIfNull(productId);
        ProductId = productId;
    }

    public ErrorOr<Success> PickProduct()
    {
        if (ProductId is null)
            return Error.Failure("Shelf doesn't currently contain a product");

        ProductId = null;

        return new Success();
    }
}