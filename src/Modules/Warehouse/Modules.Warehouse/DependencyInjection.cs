using Microsoft.Extensions.DependencyInjection;
using Modules.Warehouse.Application.Common.Behaviours;
using Modules.Warehouse.Domain.Products;
using Modules.Warehouse.Features.Products;

namespace Modules.Warehouse;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(applicationAssembly);
            config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });

        services.AddTransient<IProductRepository, ProductRepository>();

        return services;
    }
}
