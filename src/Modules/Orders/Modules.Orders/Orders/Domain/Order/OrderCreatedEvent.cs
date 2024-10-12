using Common.SharedKernel.Domain.Ids;
using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Orders.Orders.Domain.Order;

internal record OrderCreatedEvent(OrderId OrderId, CustomerId CustomerId) : IDomainEvent
{
    public static OrderCreatedEvent Create(Order order) => new(order.Id, order.CustomerId);
}
