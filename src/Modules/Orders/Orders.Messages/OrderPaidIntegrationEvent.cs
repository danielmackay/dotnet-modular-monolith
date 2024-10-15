using Common.SharedKernel.Domain.Ids;
using MediatR;

namespace Orders.Messages;

public record OrderPaidIntegrationEvent(Guid OrderId, IEnumerable<ProductId> ProductIds) : INotification;