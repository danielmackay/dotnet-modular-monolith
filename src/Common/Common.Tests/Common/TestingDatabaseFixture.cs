using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Catalog.Common.Persistence;
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

        var catalogDb = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await catalogDb.Database.MigrateAsync();

        var warehouseDb = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        await warehouseDb.Database.MigrateAsync();

        // NOTE: If there are any tables you want to skip being reset, they can be configured here
        _checkpoint = await Respawner.CreateAsync(ConnectionString);
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