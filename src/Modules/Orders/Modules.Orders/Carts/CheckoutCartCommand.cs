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
using Modules.Orders.Carts.Domain;
using Modules.Orders.Common.Persistence;

namespace Modules.Orders.Carts;

public static class CheckoutCartCommand
{
    public record Request(Guid CartId, Guid CustomerId) : IRequest<ErrorOr<Response>>;

    public record Response(Guid OrderId);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/carts/checkout",
                    async (Request request, ISender sender) =>
                    {
                        var response = await sender.Send(request);
                        return response.IsError ? response.Problem() : TypedResults.Ok(response.Value);
                    })
                .WithName("CheckoutCart")
                .WithTags("Orders")
                .ProducesPost()
                .WithOpenApi();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.CartId)
                .NotEmpty();

            RuleFor(r => r.CustomerId)
                .NotEmpty();
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
            var cartId = new CartId(request.CartId);
            var spec = new CartByIdSpec(cartId);
            var carts = await _dbContext.Carts
                .WithSpecification(spec)
                .FirstOrDefaultAsync(cancellationToken);

            if (carts is null)
                return Error.NotFound();

            // TODO: Validate customer exists
            var customerId = new CustomerId(request.CustomerId);

            var order = OrderFactory.Checkout(carts, customerId);

            _dbContext.Orders.Add(order);



            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response(order.Id.Value);
        }
    }
}