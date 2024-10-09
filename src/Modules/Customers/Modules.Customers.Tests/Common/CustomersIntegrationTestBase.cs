using Common.Tests.Common;
using Modules.Customers.Common.Persistence;
using Xunit.Abstractions;

namespace Modules.Customers.Tests.Common;

// ReSharper disable once ClassNeverInstantiated.Global
public class CustomersDatabaseFixture : TestingDatabaseFixture;

[Collection(CustomersFixtureCollection.Name)]
public abstract class CustomersIntegrationTestBase(
    CustomersDatabaseFixture fixture,
    ITestOutputHelper output)
    : IntegrationTestBase<CustomersDbContext>(fixture, output);

[CollectionDefinition(Name)]
public class CustomersFixtureCollection : ICollectionFixture<CustomersDatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.

    public const string Name = nameof(CustomersFixtureCollection);
}
