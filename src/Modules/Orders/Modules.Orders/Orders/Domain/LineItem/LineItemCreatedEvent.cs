using Common.SharedKernel.Domain.Interfaces;
using Modules.Orders.Orders.Domain.Order;

namespace Modules.Orders.Orders.Domain.LineItem;

internal record LineItemCreatedEvent(LineItemId LineItemId, OrderId Order) : IDomainEvent
{
    public LineItemCreatedEvent(LineItem lineItem) : this(lineItem.Id, lineItem.OrderId) { }

    public static LineItemCreatedEvent Create(LineItem lineItem) => new(lineItem.Id, lineItem.OrderId);
}
