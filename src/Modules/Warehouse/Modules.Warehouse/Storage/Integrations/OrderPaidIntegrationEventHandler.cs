using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.Common.Persistence;
using Orders.Messages;
using Throw;

namespace Modules.Warehouse.Storage.Integrations;

internal static class OrderPaidIntegrationEventHandler
{
    public class Handler : INotificationHandler<OrderPaidIntegrationEvent>
    {
        private readonly WarehouseDbContext _dbContext;

        public Handler(WarehouseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(OrderPaidIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var shelves = await _dbContext.Shelves
                .Where(s => notification.ProductIds.Contains(s.ProductId))
                .ToListAsync(cancellationToken);

            // Pick order ready for shipping
            foreach (var shelf in shelves)
            {
                var result = shelf.PickProduct();
                result.Throw().IfTrue(e => e.IsError);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}