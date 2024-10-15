using Common.SharedKernel.Discovery;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Modules.Orders.Common.Persistence;
using System.Reflection;

namespace Modules.Orders;

public static class OrdersModule
{
    private static readonly Assembly _module = typeof(OrdersModule).Assembly;

    public static void AddOrders(this IHostApplicationBuilder builder)
    {
        builder.AddPersistence();
        builder.Services.AddInfrastructure();
        builder.Services.AddValidatorsFromAssembly(_module);
    }

    // TODO: Refactor to REPR pattern
    public static void UseOrders(this WebApplication app)
    {
        app.MapGet("/api/orders", () =>
            {
                var orders = Enumerable.Range(1, 5).Select(index => new OrderDto
                (
                    $"Order {index}",
                    $"Order {index} description"
                ));

                return orders;
            })
            .WithName("GetOrders")
            .WithTags("Orders")
            .WithOpenApi();

        app.DiscoverEndpoints(_module);
    }
}

public record OrderDto(string Name, string Description);