using Ardalis.Specification.EntityFrameworkCore;
using Common.SharedKernel;
using Common.SharedKernel.Api;
using Common.SharedKernel.Discovery;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Common.Persistence;
using Modules.Catalog.Products.Domain;
using Request = Modules.Catalog.Messages.GetProductQuery.Request;
using Response = Modules.Catalog.Messages.GetProductQuery.Response;

namespace Modules.Catalog.Products.Integrations;

public static class GetProductQuery
{
    // public record Request(Guid ProductId) : IRequest<ErrorOr<Response>>;
    //
    // public record Response(string Name, Guid Id, string Sku, decimal Price, List<CategoryDto> Categories);
    //
    // public record CategoryDto(Guid Id, string Name);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/products/{productId:guid}",
                    async (Guid productId, ISender sender) =>
                    {
                        var request = new Request(productId);
                        var response = await sender.Send(request);
                        return response.IsError ? response.Problem() : TypedResults.Ok(response.Value);
                    })
                .WithName("GetProduct")
                .WithTags("Catalog")
                .ProducesGet<Response>()
                .WithOpenApi();
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Response>>
    {
        private readonly CatalogDbContext _dbContext;

        public Handler(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var productId = new ProductId(request.ProductId);
            var product = await _dbContext.Products
                .WithSpecification(new ProductByIdSpec(productId))
                .Select(p => new Response(p.Name, p.Id.Value, p.Sku, p.Price.Amount,
                    p.Categories.Select(c => new Messages.GetProductQuery.CategoryDto(c.Id.Value, c.Name)).ToList()))
                .FirstOrDefaultAsync(cancellationToken);

            if (product is null)
                return ProductErrors.NotFound;

            return product;
        }
    }
}