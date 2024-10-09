using Common.SharedKernel.Persistence.Interceptors;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Warehouse.Common.Persistence.Interceptors;

namespace Modules.Customers.Common.Persistence;

internal static class DepdendencyInjection
{
    internal static void AddPersistence(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<CustomersDbContext>("warehouse",
            null,
            options =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                options.AddInterceptors(
                    serviceProvider.GetRequiredService<EntitySaveChangesInterceptor>(),
                    serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>());
                options.UseExceptionProcessor();
            });

        builder.Services.AddScoped<EntitySaveChangesInterceptor>();
        builder.Services.AddScoped<DispatchDomainEventsInterceptor>();
        // services.AddScoped<OutboxInterceptor>();
    }

    public static IApplicationBuilder UseInfrastructureMiddleware(this IApplicationBuilder app)
    {
        return app;
    }
}
