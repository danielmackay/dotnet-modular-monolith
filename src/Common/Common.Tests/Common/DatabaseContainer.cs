using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;

namespace Common.Tests.Common;

/// <summary>
/// Wrapper for MSSQL container
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

    public string? ConnectionString { get; private set; }

    public async Task InitializeAsync(ILogger logger)
    {
        logger.LogInformation("Starting SQL edge container");

        try
        {
            await _container.StartAsync();
            ConnectionString = _container.GetConnectionString();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to start SQL edge container");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}