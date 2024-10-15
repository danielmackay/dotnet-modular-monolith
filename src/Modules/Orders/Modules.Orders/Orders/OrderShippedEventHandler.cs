using MediatR;
using Modules.Orders.Orders.Domain.Orders;
using Orders.Messages;

namespace Modules.Orders.Orders;


internal static class OrderShippedEventHandler
{
    public class Handler : INotificationHandler<OrderShippedEvent>
    {
        private readonly IPublisher _publisher;

        public Handler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(OrderShippedEvent notification, CancellationToken cancellationToken)
        {
            var lineItems = notification.Order.LineItems
                .Select(li => new ShippedLineItemDto(li.ProductId, li.Quantity))
                .ToList();

            var integrationEvent = new OrderShippedIntegrationEvent(notification.Order.Id.Value, lineItems);
            await _publisher.Publish(integrationEvent, cancellationToken);
        }
    }
}