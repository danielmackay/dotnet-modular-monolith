using Common.SharedKernel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Modules.Warehouse.Features.Products.Commands.CreateProduct;
using Modules.Warehouse.Features.Products.Queries.GetProducts;

namespace Modules.Warehouse.Features.Products.Endpoints;

internal static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app
            .MapGroup("api/products")
            .WithTags("Warehouse")
            .WithOpenApi();

        group
            .MapGet("/", async (ISender sender, CancellationToken ct) => await sender.Send(new GetProductsQuery(), ct))
            .WithName("GetProducts")
            .ProducesGet<ProductDto[]>();

        group
            .MapPost("/", async (ISender sender, CreateProductCommand command, CancellationToken ct) => await sender.Send(command, ct))
            .WithName("CreateProduct")
            .ProducesPost();
    }
}
