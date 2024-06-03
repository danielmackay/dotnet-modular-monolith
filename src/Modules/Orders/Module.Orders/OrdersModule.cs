using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Module.Orders;

public static class OrdersModule
{
    public static void AddOrders(this IServiceCollection services)
    {
    }

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
    }
}

record OrderDto(string Name, string Description);
