using Common.SharedKernel.Domain.Ids;
using MediatR;

namespace Orders.Messages;

public record OrderShippedIntegrationEvent(Guid OrderId, IEnumerable<ShippedLineItemDto> LineItems) : INotification;

public record ShippedLineItemDto(ProductId ProductId, int Quantity);