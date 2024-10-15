using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Orders.Orders.Domain.LineItems;

internal record LineItemId(Guid Value) : IStronglyTypedId<Guid>
{
    internal LineItemId() : this(Uuid.Create())
    {
    }
}