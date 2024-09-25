using Database.Initialisers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Catalog;
using Modules.Catalog.Common.Persistence;
using Modules.Warehouse;
using Modules.Warehouse.Common.Persistence;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    services.AddSingleton(TimeProvider.System);

    var conn = context.Configuration.GetConnectionString("Warehouse");

    services.AddDbContext<WarehouseDbContext>(options =>
    {
        options.UseSqlServer(context.Configuration.GetConnectionString("Warehouse"), opt =>
        {
            opt.MigrationsAssembly(typeof(WarehouseModule).Assembly.FullName);
        });
    });

    services.AddDbContext<CatalogDbContext>(options =>
    {
        options.UseSqlServer(context.Configuration.GetConnectionString("Catalog"), opt =>
        {
            opt.MigrationsAssembly(typeof(CatalogModule).Assembly.FullName);
        });
    });

    services.AddScoped<WarehouseDbContextInitialiser>();
    services.AddScoped<CatalogDbContextInitialiser>();
});

var app = builder.Build();
app.Start();

// Initialise and seed database
using var scope = app.Services.CreateScope();

Console.WriteLine("Waiting for SQL Server...");
await Task.Delay(5000);

var warehouse = scope.ServiceProvider.GetRequiredService<WarehouseDbContextInitialiser>();
await warehouse.InitializeAsync();
var warehouseProducts = await warehouse.SeedAsync();

var catalog = scope.ServiceProvider.GetRequiredService<CatalogDbContextInitialiser>();
await catalog.InitializeAsync();
await catalog.SeedAsync(warehouseProducts);
