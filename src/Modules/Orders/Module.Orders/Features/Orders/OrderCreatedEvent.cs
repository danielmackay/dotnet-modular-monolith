using Common.SharedKernel.Domain.Base;
using Module.Orders.Features.Customers;

namespace Module.Orders.Features.Orders;

public record OrderCreatedEvent(OrderId OrderId, CustomerId CustomerId) : DomainEvent
{
    public static OrderCreatedEvent Create(Order order) => new(order.Id, order.CustomerId);
}