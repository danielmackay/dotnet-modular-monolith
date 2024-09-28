using MigrationService.Initializers;
using OpenTelemetry.Trace;
using System.Diagnostics;

// using SupportTicketApi.Data.Contexts;
// using SupportTicketApi.Data.Models;

namespace MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var warehouseInitializer = scope.ServiceProvider.GetRequiredService<WarehouseDbContextInitializer>();

            await warehouseInitializer.EnsureDatabaseAsync(cancellationToken);
            await warehouseInitializer.RunMigrationAsync(cancellationToken);
            await warehouseInitializer.SeedDataAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }


}
