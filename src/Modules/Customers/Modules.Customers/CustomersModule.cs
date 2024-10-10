using Common.SharedKernel.Discovery;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Customers.Common.Persistence;
using System.Reflection;

namespace Modules.Customers;

public static class CustomersModule
{
    private static readonly Assembly _module = typeof(CustomersModule).Assembly;

    public static void AddCustomers(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddValidatorsFromAssembly(_module);

        builder.AddPersistence();
    }

    public static void UseCustomers(this WebApplication app)
    {
        app.UseInfrastructureMiddleware();

        app.DiscoverEndpoints(_module);
    }
}