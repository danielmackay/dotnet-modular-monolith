using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Products.UseCases;
using Modules.Warehouse.Storage.UseCases;

namespace Modules.Warehouse;

public static class WarehouseModule
{
    public static void AddWarehouse(this IHostApplicationBuilder builder)
    {
        var applicationAssembly = typeof(WarehouseModule).Assembly;

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddValidatorsFromAssembly(applicationAssembly);

        builder.AddPersistence();
    }

    public static void UseWarehouse(this WebApplication app)
    {
        app.UseInfrastructureMiddleware();

        // TODO: Consider source generation or reflection for endpoint mapping
        CreateAisleCommand.Endpoint.MapEndpoint(app);
        CreateProductCommand.Endpoint.MapEndpoint(app);
        AllocateStorageCommand.Endpoint.MapEndpoint(app);
        GetItemLocationQuery.Endpoint.MapEndpoint(app);
    }
}
