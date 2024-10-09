using MigrationService.Initializers;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<Worker> logger) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            logger.LogInformation("Waiting for SQL Server to be ready");
            await Task.Delay(10_000, cancellationToken);

            using var scope = serviceProvider.CreateScope();
            var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

            var warehouseInitializer = scope.ServiceProvider.GetRequiredService<WarehouseDbContextInitializer>();
            await warehouseInitializer.EnsureDatabaseAsync(cancellationToken);
            await warehouseInitializer.RunMigrationAsync(cancellationToken);

            var catalogInitializer = scope.ServiceProvider.GetRequiredService<CatalogDbContextInitializer>();
            await catalogInitializer.EnsureDatabaseAsync(cancellationToken);
            await catalogInitializer.RunMigrationAsync(cancellationToken);

            if (environment.IsDevelopment())
            {
                var products = await warehouseInitializer.SeedDataAsync(cancellationToken);
                await catalogInitializer.SeedDataAsync(products, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }
}
