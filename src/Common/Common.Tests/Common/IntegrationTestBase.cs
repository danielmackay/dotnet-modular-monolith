using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Common.Tests.Common;

/// <summary>
/// Integration tests inherit from this to access helper classes
/// </summary>

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly IServiceScope _scope;

    private readonly TestingDatabaseFixture _fixture;

    protected IMediator Mediator { get; }

    protected IntegrationTestBase(TestingDatabaseFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.SetOutput(output);

        _scope = _fixture.ScopeFactory.CreateScope();
        Mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    /// <summary>
    /// Gets called between each test to reset the state of the database
    /// </summary>
    public async Task InitializeAsync()
    {
        await _fixture.ResetState();
    }

    protected HttpClient GetAnonymousClient() => _fixture.Factory.AnonymousClient.Value;

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }
}