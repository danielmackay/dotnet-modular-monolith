using Common.SharedKernel.Persistence.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Warehouse.Common.Middleware;
using Modules.Warehouse.Common.Persistence.Interceptors;

namespace Modules.Warehouse.Common.Persistence;

internal static class DepdendencyInjection
{
    internal static void AddPersistence(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<WarehouseDbContext>("warehouse",
            null,
            options =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                options.AddInterceptors(
                    serviceProvider.GetRequiredService<EntitySaveChangesInterceptor>(),
                    serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>());
            });

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


        builder.Services.AddScoped<EntitySaveChangesInterceptor>();
        builder.Services.AddScoped<DispatchDomainEventsInterceptor>();
        // services.AddScoped<OutboxInterceptor>();
    }

    public static IApplicationBuilder UseInfrastructureMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}
