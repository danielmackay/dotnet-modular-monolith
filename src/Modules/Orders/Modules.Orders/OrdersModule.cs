using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Modules.Orders.Carts;
using Modules.Orders.Common.Persistence;

namespace Modules.Orders;

public static class OrdersModule
{
    public static void AddOrders(this IHostApplicationBuilder builder)
    {
        builder.AddPersistence();
        builder.Services.AddValidatorsFromAssembly(typeof(OrdersModule).Assembly);
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

        AddProductToCartCommand.Endpoint.MapEndpoint(app);
    }
}

public record OrderDto(string Name, string Description);
