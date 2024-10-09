using Common.SharedKernel.Persistence.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Warehouse.Common.Persistence.Interceptors;

namespace Modules.Customers.Common.Persistence;

internal static class DepdendencyInjection
{
    internal static void AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Customers");
        services.AddDbContext<CustomersDbContext>(options =>
        {
            options.UseSqlServer(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(CustomersModule).Assembly.FullName);
                builder.EnableRetryOnFailure(3);
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
        return app;
    }
}
