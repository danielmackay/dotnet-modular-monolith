using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Customers.Customers.Domain;

internal record CustomerId(Guid Value) : IStronglyTypedId<Guid>
{
    internal CustomerId() : this(Uuid.Create())
    {
    }
}
