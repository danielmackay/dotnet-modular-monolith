using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Customers.Common.Persistence;
using Modules.Customers.Customers.UseCases;

namespace Modules.Customers;

public static class CustomersModule
{
    public static void AddCustomers(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssembly = typeof(CustomersModule).Assembly;

        services.AddHttpContextAccessor();

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddPersistence(configuration);
    }

    public static void UseCustomers(this WebApplication app)
    {
        app.UseInfrastructureMiddleware();

        // TODO: Consider source generation or reflection for endpoint mapping
        RegisterCustomerCommand.Endpoint.MapEndpoint(app);
    }
}
