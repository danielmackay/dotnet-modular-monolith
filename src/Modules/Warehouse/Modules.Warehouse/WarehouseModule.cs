using Common.SharedKernel.Discovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Warehouse.Common.Persistence;
using System.Reflection;

namespace Modules.Warehouse;

public static class WarehouseModule
{
    private static readonly Assembly _module = typeof(WarehouseModule).Assembly;

    public static void AddWarehouse(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddValidatorsFromAssembly(_module);

        builder.AddPersistence();
    }

    public static void UseWarehouse(this WebApplication app)
    {
        app.UseInfrastructureMiddleware();
        app.DiscoverEndpoints(_module);
    }
}