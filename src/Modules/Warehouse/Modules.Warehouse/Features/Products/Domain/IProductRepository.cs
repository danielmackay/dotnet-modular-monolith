namespace Modules.Warehouse.Features.Products.Domain;

internal interface IProductRepository
{
    public Task<bool> SkuExistsAsync(Sku sku);

    public bool SkuExists(Sku sku);
}
