using Microsoft.Extensions.Logging;
using Modules.Warehouse.Products.Domain;

namespace Modules.Warehouse.Products.Events;

internal class FakeSupplierService : ISupplierService
{
    private readonly ILogger<FakeSupplierService> _logger;

    public FakeSupplierService(ILogger<FakeSupplierService> logger)
    {
        _logger = logger;
    }

    public Task<string> Order(Product product, int quantity)
    {
        _logger.LogInformation("Ordering {Product} {Quantity}", product.Id, quantity);
        return Task.FromResult(Uuid.Create().ToString());
    }
}

internal interface ISupplierService
{
    internal Task<string> Order(Product product, int quantity);
}