using Microsoft.Extensions.DependencyInjection;
using Modules.Warehouse.Features.Products;
using Modules.Warehouse.Features.Products.Domain;

namespace Modules.Warehouse;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(applicationAssembly);

        // TODO: Check we can call this multiple times
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(applicationAssembly);
        });

        services.AddTransient<IProductRepository, ProductRepository>();

        return services;
    }
}
