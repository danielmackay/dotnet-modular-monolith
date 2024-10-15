using Common.SharedKernel.Persistence.Interceptors;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Orders.Orders.Infrastructure;
using Modules.Warehouse.Common.Persistence.Interceptors;

namespace Modules.Orders.Common.Persistence;

internal static class DependencyInjection
{
    internal static void AddPersistence(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<OrdersDbContext>("warehouse",
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

    internal static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, FakePaymentService>();
    }

    public static IApplicationBuilder UseInfrastructureMiddleware(this IApplicationBuilder app)
    {
        // TODO: Will need to add this when any events are fired
        // app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}