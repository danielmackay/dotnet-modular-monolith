using Common.Tests.Common;
using Microsoft.Extensions.DependencyInjection;
using Modules.Catalog.Common.Persistence;
using Modules.Orders.Common.Persistence;
using Xunit.Abstractions;

namespace Modules.Orders.Tests.Common;

// ReSharper disable once ClassNeverInstantiated.Global
public class OrdersDatabaseFixture : TestingDatabaseFixture;

[Collection(OrdersFixtureCollection.Name)]
public abstract class OrdersIntegrationTestBase : IntegrationTestBase
{
    protected DatabaseFacade<OrdersDbContext> OrdersDb;
    protected DatabaseFacade<CatalogDbContext> CatalogDb;

    protected OrdersIntegrationTestBase(OrdersDatabaseFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
        var scope = fixture.ScopeFactory.CreateScope();

        var ordersDbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        OrdersDb = new DatabaseFacade<OrdersDbContext>(ordersDbContext);

        var catalogDbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        CatalogDb = new DatabaseFacade<CatalogDbContext>(catalogDbContext);
    }
}

[CollectionDefinition(Name)]
public class OrdersFixtureCollection : ICollectionFixture<OrdersDatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.

    public const string Name = nameof(OrdersFixtureCollection);
}