using Common.Tests.Common;
using Microsoft.Extensions.DependencyInjection;
using Modules.Catalog.Common.Persistence;
using Modules.Orders.Common.Persistence;
using Xunit.Abstractions;

namespace Modules.Orders.Tests.Common;

// ReSharper disable once ClassNeverInstantiated.Global
public class OrdersDatabaseFixture : TestingDatabaseFixture, IAsyncLifetime
{
    private IServiceScope _scope;

    public CatalogDbContext CatalogDbContext { get; private set; }

    public new async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _scope = ScopeFactory.CreateScope();
        CatalogDbContext = _scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await CatalogDbContext.DisposeAsync();
        _scope.Dispose();
    }
}

[Collection(OrdersFixtureCollection.Name)]
public abstract class OrdersIntegrationTestBase(
    OrdersDatabaseFixture fixture,
    ITestOutputHelper output)
    : IntegrationTestBase<OrdersDbContext>(fixture, output);

[CollectionDefinition(Name)]
public class OrdersFixtureCollection : ICollectionFixture<OrdersDatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.

    public const string Name = nameof(OrdersFixtureCollection);
}
