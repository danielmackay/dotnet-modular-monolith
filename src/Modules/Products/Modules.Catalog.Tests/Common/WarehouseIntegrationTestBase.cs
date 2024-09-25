using Common.Tests.Common;
using Modules.Catalog.Common.Persistence;
using Xunit.Abstractions;

namespace Modules.Catalog.Tests.Common;

// ReSharper disable once ClassNeverInstantiated.Global
public class CatalogDatabaseFixture : TestingDatabaseFixture;

[Collection(CatalogFixtureCollection.Name)]
public abstract class CatalogIntegrationTestBase(
    CatalogDatabaseFixture fixture,
    ITestOutputHelper output)
    : IntegrationTestBase<CatalogDbContext>(fixture, output);

[CollectionDefinition(Name)]
public class CatalogFixtureCollection : ICollectionFixture<CatalogDatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.

    public const string Name = nameof(CatalogFixtureCollection);
}