using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Features.Products;
using Modules.Warehouse.Features.Products.Domain;
using Modules.Warehouse.Features.Products.Endpoints;

namespace Modules.Warehouse;

public static class WarehouseModule
{
    public static void AddWarehouse(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssembly = typeof(WarehouseModule).Assembly;

        services.AddValidatorsFromAssembly(applicationAssembly);

        // TODO: Check we can call this multiple times
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(applicationAssembly);
        });

        // Todo: Move to feature DI
        services.AddTransient<IProductRepository, ProductRepository>();
    }

    public static async Task UseWarehouse(this WebApplication app)
    {
        // TODO: Refactor to up.ps1
        if (app.Environment.IsDevelopment())
        {
            // Initialise and seed database
            using var scope = app.Services.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<WarehouseDbContextInitializer>();
            await initializer.InitializeAsync();
            await initializer.SeedAsync();
        }

        // TODO: Move to feature DI
        app.MapProductEndpoints();
    }
}
