using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Orders.Orders.Domain.LineItem;

internal record LineItemId(Guid Value) : IStronglyTypedId<Guid>
{
    internal LineItemId() : this(Uuid.Create())
    {
    }
}