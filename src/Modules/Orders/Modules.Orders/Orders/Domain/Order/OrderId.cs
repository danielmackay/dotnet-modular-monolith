using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Orders.Orders.Domain.Order;

internal record OrderId(Guid Value) : IStronglyTypedId<Guid>
{
    internal OrderId() : this(Uuid.Create())
    {
    }
}