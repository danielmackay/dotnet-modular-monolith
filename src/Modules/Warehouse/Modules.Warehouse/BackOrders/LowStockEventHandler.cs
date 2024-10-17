using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.BackOrders.Domain;
using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Products.Domain;

namespace Modules.Warehouse.BackOrders;

internal static class LowStockEventHandler
{
    internal class Handler : INotificationHandler<LowStockEvent>
    {
        private readonly WarehouseDbContext _dbContext;
        private readonly ISupplierService _supplierService;

        public Handler(WarehouseDbContext dbContext, ISupplierService supplierService)
        {
            _dbContext = dbContext;
            _supplierService = supplierService;
        }

        public async Task Handle(LowStockEvent notification, CancellationToken cancellationToken)
        {
            var spec = new ProductByIdSpec(notification.Product.Id);
            var product = await _dbContext.Products
                .WithSpecification(spec)
                .FirstAsync(cancellationToken);

            if (product is null)
                throw new InvalidOperationException("Cannot find product");

            var existingBackOrder = await _dbContext.BackOrders.AnyAsync(bo =>
                bo.ProductId == notification.Product.Id && bo.Status == BackOrderStatus.Pending, cancellationToken: cancellationToken);

            if (existingBackOrder)
                return;

            var backOrder = BackOrder.Create(product.Id);
            var reference = await _supplierService.Order(product, backOrder.QuantityOrdered);
            backOrder.UpdateReference(reference);
            _dbContext.BackOrders.Add(backOrder);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}