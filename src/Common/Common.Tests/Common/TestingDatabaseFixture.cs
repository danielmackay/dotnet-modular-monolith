using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Catalog.Common.Persistence;
using Modules.Customers.Common.Persistence;
using Modules.Orders.Common.Persistence;
using Modules.Warehouse.Common.Persistence;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace Common.Tests.Common;

/// <summary>
/// Initializes and resets the database before and after each test
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class TestingDatabaseFixture : IAsyncLifetime
{
    private string ConnectionString => Factory.Database.ConnectionString!;

    private Respawner _checkpoint = default!;

    public IServiceScopeFactory ScopeFactory { get; private set; } = null!;

    public WebApiTestFactory Factory { get; } = new();

    public async Task InitializeAsync()
    {
        // Initialize DB Container
        await Factory.Database.InitializeAsync();
        ScopeFactory = Factory.Services.GetRequiredService<IServiceScopeFactory>();

        // Create and seed databases
        using var scope = ScopeFactory.CreateScope();
        await MigrateDatabase<CatalogDbContext>(scope);
        await MigrateDatabase<WarehouseDbContext>(scope);
        await MigrateDatabase<CustomersDbContext>(scope);
        await MigrateDatabase<OrdersDbContext>(scope);

        // NOTE: If there are any tables you want to skip being reset, they can be configured here
        _checkpoint = await Respawner.CreateAsync(ConnectionString);
    }

    private static async Task MigrateDatabase<T>(IServiceScope scope) where T : DbContext
    {
        var db = scope.ServiceProvider.GetRequiredService<T>();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await Factory.Database.DisposeAsync();
    }

    public async Task ResetState()
    {
        await _checkpoint.ResetAsync(ConnectionString);
    }

    public void SetOutput(ITestOutputHelper output) => Factory.Output = output;
}