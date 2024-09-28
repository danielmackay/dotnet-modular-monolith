using Common.SharedKernel.Persistence.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Warehouse.Common.Middleware;
using Modules.Warehouse.Common.Persistence.Interceptors;

namespace Modules.Warehouse.Common.Persistence;

internal static class DepdendencyInjection
{
    internal static void AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        // TODO: Fix up
        // var connectionString = config.GetConnectionString("Warehouse");
        // services.AddDbContext<WarehouseDbContext>(options =>
        // {
        //     options.UseSqlServer(connectionString, builder =>
        //     {
        //         builder.MigrationsAssembly(typeof(WarehouseModule).Assembly.FullName);
        //         // builder.EnableRetryOnFailure();
        //     });
        //
        //     var serviceProvider = services.BuildServiceProvider();
        //
        //     options.AddInterceptors(
        //         serviceProvider.GetRequiredService<EntitySaveChangesInterceptor>(),
        //         serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>());
        // });


        services.AddScoped<EntitySaveChangesInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();
        // services.AddScoped<OutboxInterceptor>();
    }

    public static IApplicationBuilder UseInfrastructureMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}
