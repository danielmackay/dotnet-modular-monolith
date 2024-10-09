using Common.SharedKernel.Persistence.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Warehouse.Common.Persistence.Interceptors;

namespace Modules.Catalog.Common.Persistence;

internal static class DependencyInjection
{
    internal static void AddPersistence(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<CatalogDbContext>("catalog",
            null,
            options =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                options.AddInterceptors(
                    serviceProvider.GetRequiredService<EntitySaveChangesInterceptor>(),
                    serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>());
            });

        builder.Services.AddScoped<EntitySaveChangesInterceptor>();
        builder.Services.AddScoped<DispatchDomainEventsInterceptor>();
        // services.AddScoped<OutboxInterceptor>();
    }

    public static IApplicationBuilder UseInfrastructureMiddleware(this IApplicationBuilder app)
    {
        // TODO: Will need to add this when any events are fired
        // app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}
