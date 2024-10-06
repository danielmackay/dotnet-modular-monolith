namespace Common.SharedKernel.Domain.Ids;

public record ProductId(Guid Value)
{
    public ProductId() : this(Uuid.Create())
    {
    }
}
