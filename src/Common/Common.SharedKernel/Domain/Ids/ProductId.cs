using Common.SharedKernel.Domain.Interfaces;

namespace Common.SharedKernel.Domain.Ids;

public record ProductId(Guid Value) : IStronglyTypedId<Guid>
{
    public ProductId() : this(Uuid.Create())
    {
    }
}
