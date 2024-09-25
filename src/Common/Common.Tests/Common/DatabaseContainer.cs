using Polly;
using Testcontainers.MsSql;

namespace Common.Tests.Common;

/// <summary>
/// Wrapper for MS SQL container
/// </summary>
public class DatabaseContainer : IAsyncDisposable
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
        .WithName($"BizCover-IntegrationTests-{Guid.NewGuid()}")
        .WithPassword("Password123")
        .WithPortBinding(1433, true)
        .WithAutoRemove(true)
        .Build();

    private const int MaxRetries = 5;

    public string? ConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        await StartWithRetry();
        ConnectionString = _container.GetConnectionString();
    }

    private async Task StartWithRetry()
    {
        // NOTE: For some reason the container sometimes fails to start up.  Add in a retry to protect against this
        var policy = Policy.Handle<Exception>()
            .WaitAndRetry(MaxRetries, _ => TimeSpan.FromMilliseconds(2000));

        await policy.Execute(async () => { await _container.StartAsync(); });
    }

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}