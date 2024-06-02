using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Warehouse.Application;
using Modules.Warehouse.Features.Products;
using Modules.Warehouse.Features.Products.Domain;
using Modules.Warehouse.Infrastructure;
using Modules.Warehouse.Infrastructure.Persistence;

namespace Modules.Warehouse.Endpoints;

public static class WarehouseModule
{
    public static void AddWarehouseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
    }

    public static async Task UseWarehouseModule(this WebApplication app)
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

        app.MapProductEndpoints();
    }

    private static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(applicationAssembly);

        // TODO: Check we can call this multiple times
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(applicationAssembly);
        });

        services.AddTransient<IProductRepository, ProductRepository>();

        return services;
    }
}
