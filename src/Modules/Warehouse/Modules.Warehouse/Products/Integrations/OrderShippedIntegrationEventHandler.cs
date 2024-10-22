using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.Common.Persistence;
using Orders.Messages;

namespace Modules.Warehouse.Products.Integrations;

// TODO: Add Integration Tests
public static class OrderShippedIntegrationEventHandler
{
    public class Handler : INotificationHandler<OrderShippedIntegrationEvent>
    {
        private readonly WarehouseDbContext _dbContext;

        public Handler(WarehouseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(OrderShippedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var productIds = notification.LineItems
                .Select(li => li.ProductId)
                .ToList();

            var products = await _dbContext.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            var tuples = products.Join(
                notification.LineItems,
                p => p.Id,
                li => li.ProductId,
                (product, lineItem) => (product, lineItem));

            foreach (var tuple in tuples)
            {
                var result = tuple.product.RemoveStock(tuple.lineItem.Quantity);
                ThrowIfEqual(result.IsError, true);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}