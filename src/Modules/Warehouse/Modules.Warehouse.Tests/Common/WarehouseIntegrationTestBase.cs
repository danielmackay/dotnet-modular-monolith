using Common.Tests.Common;
using Microsoft.Extensions.DependencyInjection;
using Modules.Warehouse.Common.Persistence;
using Xunit.Abstractions;

namespace Modules.Warehouse.Tests.Common;

// ReSharper disable once ClassNeverInstantiated.Global
public class WarehouseDatabaseFixture : TestingDatabaseFixture;

[Collection(WarehouseFixtureCollection.Name)]
public abstract class WarehouseIntegrationTestBase : IntegrationTestBase
{
    protected DatabaseFacade<WarehouseDbContext> Database;

    protected WarehouseIntegrationTestBase(WarehouseDatabaseFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
        var scope = fixture.ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        Database = new DatabaseFacade<WarehouseDbContext>(dbContext);
    }
}

[CollectionDefinition(Name)]
public class WarehouseFixtureCollection : ICollectionFixture<WarehouseDatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.

    public const string Name = nameof(WarehouseFixtureCollection);
}