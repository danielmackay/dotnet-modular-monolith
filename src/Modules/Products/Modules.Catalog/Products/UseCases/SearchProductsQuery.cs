using Common.SharedKernel;
using Common.SharedKernel.Discovery;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Categories.Domain;
using Modules.Catalog.Common.Persistence;
using Modules.Catalog.Products.Domain;

namespace Modules.Catalog.Products.UseCases;

public static class SearchProductsQuery
{
    public record Request(string? Name, Guid? CategoryId) : IRequest<IReadOnlyList<Response>>;

    public record Response(string Name, Guid Id, string Sku, decimal Price);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/products",
                    async (string? name, Guid? categoryId, ISender sender) =>
                    {
                        var request = new Request(name, categoryId);
                        var response = await sender.Send(request);
                        return TypedResults.Ok(response);
                    })
                .WithName("SearchProducts")
                .WithTags("Catalog")
                .ProducesGet<Response>()
                .WithOpenApi();
        }
    }

    internal class Handler : IRequestHandler<Request, IReadOnlyList<Response>>
    {
        private readonly CatalogDbContext _dbContext;

        public Handler(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _dbContext.Products;

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(p => p.Name.Contains(request.Name));

            if (request.CategoryId is not null)
            {
                var categoryId = new CategoryId(request.CategoryId.Value);
                query = query.Where(p => p.Categories.Any(c => c.Id == categoryId));
            }

            var products = await query
                .Select(p => new Response(p.Name, p.Id.Value, p.Sku, p.Price.Amount))
                .ToListAsync(cancellationToken);

            return products;
        }
    }
}