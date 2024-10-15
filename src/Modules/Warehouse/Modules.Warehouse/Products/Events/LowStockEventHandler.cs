using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Products.Domain;

namespace Modules.Warehouse.Products.Events;

internal static class LowStockEventHandler
{
    internal class Handler : INotificationHandler<LowStockEvent>
    {
        private readonly WarehouseDbContext _dbContext;

        public Handler(WarehouseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(LowStockEvent notification, CancellationToken cancellationToken)
        {
            var spec = new ProductByIdSpec(notification.Product.Id);
            var product = await _dbContext.Products
                .WithSpecification(spec)
                .FirstAsync(cancellationToken);



        }
    }
}