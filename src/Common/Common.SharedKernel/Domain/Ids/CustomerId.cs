using Common.SharedKernel.Domain.Interfaces;

namespace Common.SharedKernel.Domain.Ids;

public record CustomerId(Guid Value) : IStronglyTypedId<Guid>
{
    public CustomerId() : this(Uuid.Create())
    {
    }
}