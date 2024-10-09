using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Customers.Common.Persistence;
using Modules.Customers.Customers.UseCases;

namespace Modules.Customers;

public static class CustomersModule
{
    public static void AddCustomers(this IHostApplicationBuilder builder)
    {
        var applicationAssembly = typeof(CustomersModule).Assembly;

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddValidatorsFromAssembly(applicationAssembly);

        builder.AddPersistence();
    }

    public static void UseCustomers(this WebApplication app)
    {
        app.UseInfrastructureMiddleware();

        // TODO: Consider source generation or reflection for endpoint mapping
        RegisterCustomerCommand.Endpoint.MapEndpoint(app);
    }
}
