using Common.SharedKernel;
using Common.SharedKernel.Api;
using Common.SharedKernel.Discovery;
using ErrorOr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Storage.Domain;

namespace Modules.Warehouse.Storage.UseCases;

public static class GetItemLocationQuery
{
    public record Request(Guid ProductId) : IRequest<ErrorOr<Response>>;

    public record Response(string AisleName, string BayName, string ShelfName);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/aisles/products/{productId:guid}", async (Guid productId, ISender sender) =>
                {
                    var response = await sender.Send(new Request(productId));
                    return response.IsError ? response.Problem() : TypedResults.Ok(response.Value);
                })
                .WithName("FindProductLocation")
                .WithTags("Warehouse")
                .ProducesGet<Response>()
                .WithOpenApi();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.ProductId).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Response>>
    {
        private readonly WarehouseDbContext _dbContext;

        public Handler(WarehouseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var aisles = await _dbContext.Aisles
                .WithSpecification(new GetAllAislesSpec())
                .ToListAsync(cancellationToken);

            var productId = new ProductId(request.ProductId);

            // Consider adjusting the model to make the query more efficient if needed
            foreach (var aisle in aisles)
            {
                foreach (var bay in aisle.Bays)
                {
                    foreach (var shelf in bay.Shelves)
                    {
                        if (shelf.ProductId == productId)
                            return new Response(aisle.Name, bay.Name, shelf.Name);
                    }
                }
            }

            return Error.NotFound(description: "Product not found");
        }
    }
}