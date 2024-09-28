using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;

namespace MigrationService.Initializers;

internal class WarehouseDbContextInitializer
{
    private readonly WarehouseDbContext _dbContext;

    private const int NumProducts = 20;
    private const int NumAisles = 10;
    private const int NumShelves = 5;
    private const int NumBays = 20;

    // public constructor needed for DI
    public WarehouseDbContextInitializer(WarehouseDbContext dbContext)
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

    public async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            await SeedAisles();
            await SeedProductsAsync();
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }

    private async Task SeedAisles()
    {
        if (await _dbContext.Aisles.AnyAsync())
            return;

        for (var i = 1; i <= NumAisles; i++)
        {
            var aisle = Aisle.Create($"Aisle {i}", NumBays, NumShelves);
            _dbContext.Aisles.Add(aisle);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<IReadOnlyList<Product>> SeedProductsAsync()
    {
        if (await _dbContext.Products.AnyAsync())
            return [];

        // TODO: Consider how to handle integration events that get raised and handled

        var skuFaker = new Faker<Sku>()
            .CustomInstantiator(f => Sku.Create(f.Commerce.Ean8())!);

        var faker = new Faker<Product>()
            .CustomInstantiator(f => Product.Create(f.Commerce.ProductName(), skuFaker.Generate()));

        var products = faker.Generate(NumProducts);
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        return products;
    }
}
