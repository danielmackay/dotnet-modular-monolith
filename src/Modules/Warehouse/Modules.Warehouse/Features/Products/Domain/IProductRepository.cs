namespace Modules.Warehouse.Features.Products.Domain;

public interface IProductRepository
{
    public Task<bool> SkuExistsAsync(Sku sku);

    public bool SkuExists(Sku sku);
}
