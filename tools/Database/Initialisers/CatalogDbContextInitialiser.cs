using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Categories.Domain;
using Modules.Catalog.Common.Persistence;
using Modules.Warehouse.Products.Domain;

namespace Database.Initialisers;

internal class CatalogDbContextInitialiser
{
    private readonly ILogger<CatalogDbContextInitialiser> _logger;
    private readonly CatalogDbContext _dbContext;

    private const int NumCategories = 10;

    // public constructor needed for DI
    public CatalogDbContextInitialiser(ILogger<CatalogDbContextInitialiser> logger, CatalogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    internal async Task InitializeAsync()
    {
        try
        {
            if (_dbContext.Database.IsSqlServer())
            {
                await _dbContext.Database.MigrateAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while migrating or initializing the database");
            throw;
        }
    }

    internal async Task SeedAsync(IEnumerable<Product> warehouseProducts)
    {
        var categories = await SeedCategories();
        await SeedProductsAsync(warehouseProducts, categories);
    }

    private async Task<IReadOnlyList<Category>> SeedCategories()
    {
        if (await _dbContext.Categories.AnyAsync())
            return [];

        var categoryFaker = new Faker<Category>()
            .CustomInstantiator(f => Category.Create(f.Commerce.Categories(1).First()!));

        var categories = categoryFaker.Generate(NumCategories);
        _dbContext.Categories.AddRange(categories);
        await _dbContext.SaveChangesAsync();

        return categories;
    }

    private async Task SeedProductsAsync(IEnumerable<Product> warehouseProducts, IEnumerable<Category> categories)
    {
        if (await _dbContext.Products.AnyAsync())
            return;

        var categoryFaker = new Faker<Category>()
            .CustomInstantiator(f => f.PickRandom(categories));

        // Usually integration events would propagate products to the catalog
        // However, to simplify test data seed, we'll manually pass products into the catalog
        foreach (var warehouseProduct in warehouseProducts)
        {
            var catalogProduct = Modules.Catalog.Products.Domain.Product.Create(
                warehouseProduct.Name,
                warehouseProduct.Sku.Value,
                new ProductId(warehouseProduct.Id.Value));

            var productCategory = categoryFaker.Generate();
            catalogProduct.AddCategory(productCategory);

            _dbContext.Products.Add(catalogProduct);
        }

        await _dbContext.SaveChangesAsync();
    }
}
