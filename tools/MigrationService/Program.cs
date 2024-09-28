using MigrationService;
using MigrationService.Initializers;
using Modules.Warehouse.Common.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.Services.AddScoped<WarehouseDbContextInitializer>();
builder.AddSqlServerDbContext<WarehouseDbContext>("warehouse");

var host = builder.Build();
host.Run();
