using Ardalis.Specification.EntityFrameworkCore;
using Common.SharedKernel;
using Common.SharedKernel.Api;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Common.Persistence;
using Modules.Catalog.Products.Domain;
using System.Text.Json.Serialization;

namespace Modules.Catalog.Products.UseCases;

public static class UpdateProductPriceCommand
{
    public record Request(decimal Price) : IRequest<ErrorOr<Success>>
    {
        [JsonIgnore]
        public Guid ProductId { get; set; }
    }

    public static class Endpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/products/{productId:guid}/price",
                    async (Guid productId, Request request, ISender sender) =>
                    {
                        request.ProductId = productId;
                        var response = await sender.Send(request);
                        return response.IsError ? response.Problem() : TypedResults.NoContent();
                    })
                .WithName("UpdateProductPrice")
                .WithTags("Catalog")
                .ProducesPost()
                .WithOpenApi();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.ProductId)
                .NotEmpty();

            RuleFor(r => r.Price)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Success>>
    {
        private readonly CatalogDbContext _dbContext;

        public Handler(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<Success>> Handle(Request request, CancellationToken cancellationToken)
        {
            var productId = new ProductId(request.ProductId);
            var product = await _dbContext.Products
                .WithSpecification(new ProductByIdSpec(productId))
                .FirstOrDefaultAsync(cancellationToken);

            if (product is null)
                return ProductErrors.NotFound;

            var money = Money.Create(request.Price);
            product.UpdatePrice(money);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}