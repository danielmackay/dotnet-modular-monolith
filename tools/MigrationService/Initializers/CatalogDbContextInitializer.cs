using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Modules.Catalog.Categories.Domain;
using Modules.Catalog.Common.Persistence;
using Modules.Warehouse.Products.Domain;
using ProductId = Modules.Catalog.Products.Domain.ProductId;

namespace MigrationService.Initializers;

internal class CatalogDbContextInitializer
{
    private readonly CatalogDbContext _dbContext;

     private const int NumCategories = 10;

    // public constructor needed for DI
    public CatalogDbContextInitializer(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnsureDatabaseAsync(CancellationToken cancellationToken)
    {
        var dbCreator = _dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = _dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    public async Task RunMigrationAsync(CancellationToken cancellationToken)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            await _dbContext.Database.MigrateAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }

    public async Task SeedDataAsync(IReadOnlyList<Product> products, CancellationToken cancellationToken)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            var categories = await SeedCategories();
            await SeedProductsAsync(products, categories);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
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
