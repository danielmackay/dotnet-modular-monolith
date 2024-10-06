using Common.SharedKernel;
using Common.SharedKernel.Api;
using Common.SharedKernel.Domain.Ids;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Orders.Carts.Domain;
using Modules.Orders.Common.Persistence;

namespace Modules.Orders.Carts;

public static class AddProductToCartCommand
{
    public record Request(Guid? CartId, Guid ProductId) : IRequest<ErrorOr<Success>>;

    public static class Endpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/carts",
                    async (Guid productId, Guid categoryId, ISender sender) =>
                    {
                        var request = new Request(productId, categoryId);
                        var response = await sender.Send(request);
                        return response.IsError ? response.Problem() : TypedResults.Created();
                    })
                .WithName("AddProductCategory")
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

            RuleFor(r => r.CategoryId)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Success>>
    {
        private readonly OrdersDbContext _dbContext;

        public Handler(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<Success>> Handle(Request request, CancellationToken cancellationToken)
        {
            var productId = new ProductId(request.ProductId);

            // TODO: Get product from Catalog


            if (request.CartId is null)
            {
                var cart = Cart.Create()
            }
            var cartId = new CartId(request.CartId);
            var product = await _dbContext.Products
                    .WithSpecification(new ProductByIdSpec(productId))
                    .FirstOrDefaultAsync(cancellationToken);

            if (product is null)
                return ProductErrors.NotFound;

            var categoryId = new CategoryId(request.CategoryId);
            var category =
                await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId,
                    cancellationToken: cancellationToken);

            if (category is null)
                return CategoryErrors.NotFound;

            product.AddCategory(category);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
