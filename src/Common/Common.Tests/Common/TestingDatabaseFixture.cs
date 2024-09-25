using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace Common.Tests.Common;

/// <summary>
/// Initializes and resets the database before and after each test
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class TestingDatabaseFixture<TDbContext> : IAsyncLifetime where TDbContext : DbContext
{
    private string ConnectionString => Factory.Database.ConnectionString!;

    private Respawner _checkpoint = default!;

    public IServiceScopeFactory ScopeFactory { get; private set; } = null!;

    public WebApiTestFactory<TDbContext> Factory { get; } = new();

    public async Task InitializeAsync()
    {
        ScopeFactory = Factory.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = ScopeFactory.CreateScope();

        // Initialize DB Container
        // var provider = scope.ServiceProvider.GetRequiredService<ILoggerProvider>();
        await Factory.Database.InitializeAsync();

        Console.WriteLine("FOO");

        // Create and seed database
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

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