using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Orders.Orders.Domain.Orders;

internal record OrderPaidEvent(Order Order) : IDomainEvent;