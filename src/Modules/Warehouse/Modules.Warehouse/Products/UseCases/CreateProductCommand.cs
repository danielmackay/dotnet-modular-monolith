// ReSharper disable UnusedType.Global

using Common.SharedKernel;
using Common.SharedKernel.Api;
using Common.SharedKernel.Discovery;
using ErrorOr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Products.Domain;

namespace Modules.Warehouse.Products.UseCases;

public static class CreateProductCommand
{
    public record Request(string Name, string Sku) : IRequest<ErrorOr<Response>>;

    public record Response(Guid ProductId);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/products", async (Request request, ISender sender) =>
                {
                    var response = await sender.Send(request);
                    return response.Match(TypedResults.Ok, ErrorOrExt.Problem);
                })
                .WithName("Create Product")
                .WithTags("Warehouse")
                .ProducesPost()
                .WithOpenApi();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name).NotEmpty();
            RuleFor(r => r.Sku)
                .NotEmpty()
                .Length(Sku.DefaultLength);
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
            var sku = Sku.Create(request.Sku);

            var product = Product.Create(request.Name, sku);
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response(product.Id.Value);
        }
    }
}