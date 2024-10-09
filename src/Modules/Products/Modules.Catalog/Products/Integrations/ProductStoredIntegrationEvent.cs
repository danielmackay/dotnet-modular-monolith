using MediatR;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Common.Persistence;
using Modules.Catalog.Products.Domain;

namespace Modules.Catalog.Products.Integrations;

internal class ProductStoredIntegrationEvent : INotificationHandler<Warehouse.Messages.ProductStoredIntegrationEvent>
{
    private readonly ILogger<ProductStoredIntegrationEvent> _logger;
    private readonly CatalogDbContext _dbContext;

    public ProductStoredIntegrationEvent(ILogger<ProductStoredIntegrationEvent> logger, CatalogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(Warehouse.Messages.ProductStoredIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Product stored integration event received");

        var productId = new ProductId(notification.ProductId);
        var name = notification.ProductName;
        var sku = notification.productSku;

        var product = Product.Create(name, sku, productId);

        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product stored integration event processed");

        await Task.CompletedTask;

    }
}
