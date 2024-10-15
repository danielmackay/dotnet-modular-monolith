using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Orders.Orders.Domain.Orders;

internal record OrderPaidEvent(Order Order) : IDomainEvent;

internal record OrderCreatedEvent(Order Order) : IDomainEvent;

internal record OrderShippedEvent(Order Order) : IDomainEvent;