using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;

namespace Database.Initialisers;

internal class WarehouseDbContextInitialiser
{
    private readonly ILogger<WarehouseDbContextInitialiser> _logger;
    private readonly WarehouseDbContext _dbContext;

    private const int NumProducts = 20;
    private const int NumAisles = 10;
    private const int NumShelves = 5;
    private const int NumBays = 20;

    // public constructor needed for DI
    public WarehouseDbContextInitialiser(ILogger<WarehouseDbContextInitialiser> logger, WarehouseDbContext dbContext)
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

    internal async Task<IReadOnlyList<Product>> SeedAsync()
    {
        await SeedAisles();
        return await SeedProductsAsync();
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