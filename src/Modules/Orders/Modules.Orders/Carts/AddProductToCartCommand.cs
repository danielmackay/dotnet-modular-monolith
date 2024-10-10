using Ardalis.Specification.EntityFrameworkCore;
using Common.SharedKernel;
using Common.SharedKernel.Api;
using Common.SharedKernel.Discovery;
using Common.SharedKernel.Domain.Ids;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Messages;
using Modules.Orders.Carts.Domain;
using Modules.Orders.Common.Persistence;

namespace Modules.Orders.Carts;

public static class AddProductToCartCommand
{
    public record Request(Guid? CartId, Guid ProductId, int Quantity) : IRequest<ErrorOr<Response>>;

    public record Response(Guid CartId);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/carts",
                    async (Request request, ISender sender) =>
                    {
                        var response = await sender.Send(request);
                        return response.IsError ? response.Problem() : TypedResults.Ok(response.Value);
                    })
                .WithName("AddProductToCart")
                .WithTags("Orders")
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

            RuleFor(r => r.Quantity)
                .NotEmpty()
                .GreaterThan(0);
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Response>>
    {
        private readonly OrdersDbContext _dbContext;
        private readonly IMediator _mediator;

        public Handler(OrdersDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = new GetProductQuery.Request(request.ProductId);
            var product = await _mediator.Send(query, cancellationToken);
            if (product.IsError)
                return product.Errors;

            var productId = new ProductId(product.Value.Id);
            var price = new Money(product.Value.Price);
            var quantity = request.Quantity;
            Cart? cart;

            if (request.CartId is null)
            {
                cart = Cart.Create(productId, quantity, price);
                _dbContext.Carts.Add(cart);
            }
            else
            {
                var cartId = new CartId(request.CartId.Value);
                cart = await _dbContext.Carts
                    .WithSpecification(new CartByIdSpec(cartId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (cart is null)
                    return CartErrors.NotFound;

                cart.AddItem(productId, quantity, price);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response(cart.Id.Value);
        }
    }
}