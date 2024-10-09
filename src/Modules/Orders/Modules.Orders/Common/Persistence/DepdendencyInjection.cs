using Common.SharedKernel.Persistence.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Warehouse.Common.Persistence.Interceptors;

namespace Modules.Orders.Common.Persistence;

internal static class DependencyInjection
{
    internal static void AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Catalog");
        services.AddDbContext<OrdersDbContext>(options =>
        {
            options.UseSqlServer(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(OrdersModule).Assembly.FullName);
                // builder.EnableRetryOnFailure();
            });

            var serviceProvider = services.BuildServiceProvider();

            options.AddInterceptors(
                serviceProvider.GetRequiredService<EntitySaveChangesInterceptor>(),
                serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>());
        });

        services.AddScoped<EntitySaveChangesInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();
        // services.AddScoped<OutboxInterceptor>();
    }

    public static IApplicationBuilder UseInfrastructureMiddleware(this IApplicationBuilder app)
    {
        // TODO: Will need to add this when any events are fired
        // app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}
