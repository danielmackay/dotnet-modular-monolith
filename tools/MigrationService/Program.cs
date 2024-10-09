using MigrationService;
using MigrationService.Initializers;
using Modules.Catalog.Common.Persistence;
using Modules.Warehouse.Common.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.Services.AddScoped<WarehouseDbContextInitializer>();
builder.AddSqlServerDbContext<WarehouseDbContext>("warehouse");

builder.Services.AddScoped<CatalogDbContextInitializer>();
builder.AddSqlServerDbContext<CatalogDbContext>("catalog");


var host = builder.Build();

host.Run();
