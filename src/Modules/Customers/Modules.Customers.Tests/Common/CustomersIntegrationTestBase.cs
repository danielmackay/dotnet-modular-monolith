using Common.Tests.Common;
using Microsoft.Extensions.DependencyInjection;
using Modules.Customers.Common.Persistence;
using Xunit.Abstractions;

namespace Modules.Customers.Tests.Common;

// ReSharper disable once ClassNeverInstantiated.Global
public class CustomersDatabaseFixture : TestingDatabaseFixture;

public abstract class CustomersIntegrationTestBase : IntegrationTestBase
{
    protected DatabaseFacade<CustomersDbContext> Database;

    protected CustomersIntegrationTestBase(CustomersDatabaseFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
        var scope = fixture.ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();
        Database = new DatabaseFacade<CustomersDbContext>(dbContext);
    }
}

[CollectionDefinition(Name)]
public class CustomersFixtureCollection : ICollectionFixture<CustomersDatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.

    public const string Name = nameof(CustomersFixtureCollection);
}