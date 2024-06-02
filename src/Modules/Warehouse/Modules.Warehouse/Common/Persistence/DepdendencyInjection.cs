using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Warehouse.Common.Persistence;

internal static class DepdendencyInjection
{
    internal static void AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<WarehouseDbContext>(options =>
            options.UseSqlServer(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(WarehouseModule).Assembly.FullName);
                builder.EnableRetryOnFailure();
            }));

        //services.AddSingleton<IDateTime, DateTimeService>();
        // TODO: Consider moving to up.ps1
        services.AddScoped<WarehouseDbContextInitializer>();
        // services.AddScoped<EntitySaveChangesInterceptor>();
        // services.AddScoped<DispatchDomainEventsInterceptor>();
        // services.AddScoped<OutboxInterceptor>();
    }
}
