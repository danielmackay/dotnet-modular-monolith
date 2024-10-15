using MediatR;
using Modules.Orders.Orders.Domain.Orders;
using Orders.Messages;

namespace Modules.Orders.Orders;

internal static class OrderPaidEventHandler
{
    public class Handler : INotificationHandler<OrderPaidEvent>
    {
        private readonly IPublisher _publisher;

        public Handler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(OrderPaidEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new OrderPaidIntegrationEvent(notification.Order.Id.Value, notification.Order.GetProducts());
            await _publisher.Publish(integrationEvent, cancellationToken);
        }
    }
}