using Common.Tests.Common;
using Microsoft.Extensions.DependencyInjection;
using Modules.Catalog.Common.Persistence;
using Xunit.Abstractions;

namespace Modules.Catalog.Tests.Common;

// ReSharper disable once ClassNeverInstantiated.Global
public class CatalogDatabaseFixture : TestingDatabaseFixture;

[Collection(CatalogFixtureCollection.Name)]
public abstract class CatalogIntegrationTestBase : IntegrationTestBase
{
    protected DatabaseFacade<CatalogDbContext> Database;

    protected CatalogIntegrationTestBase(CatalogDatabaseFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
        var scope = fixture.ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        Database = new DatabaseFacade<CatalogDbContext>(dbContext);
    }
}

[CollectionDefinition(Name)]
public class CatalogFixtureCollection : ICollectionFixture<CatalogDatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.

    public const string Name = nameof(CatalogFixtureCollection);
}