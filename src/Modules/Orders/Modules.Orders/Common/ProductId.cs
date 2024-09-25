namespace Modules.Orders.Common;

internal record ProductId(Guid Value)
{
    internal ProductId() : this(Uuid.Create())
    {
    }
}