using Common.SharedKernel.Domain.Base;

namespace Module.Orders.Features.Orders;

internal record OrderReadyForShippingEvent(OrderId OrderId) : DomainEvent
{
    public static OrderReadyForShippingEvent Create(Order order) => new(order.Id);
}
