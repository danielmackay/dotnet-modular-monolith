using Ardalis.Specification.EntityFrameworkCore;
using Common.SharedKernel;
using Common.SharedKernel.Api;
using Common.SharedKernel.Discovery;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Orders.Common.Persistence;
using Modules.Orders.Orders.Domain.Orders;

namespace Modules.Orders.Orders;

public static class AddPaymentCommand
{
    // TODO: Test if FromRoute parameter is correct
    public record Request([FromRoute]Guid OrderId, decimal Amount, CreditCardDto Card) : IRequest<ErrorOr<Response>>;

    public record CreditCardDto(string CardNumber, string ExpirationMonth, string ExpirationYear, string SecurityCode);

    public record Response();

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/orders/{orderId:guid}/payment",
                    async (Request request, ISender sender) =>
                    {
                        var response = await sender.Send(request);
                        return response.IsError ? response.Problem() : TypedResults.Ok(response.Value);
                    })
                .WithName("OrderPayment")
                .WithTags("Orders")
                .ProducesPost()
                .WithOpenApi();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.OrderId)
                .NotEmpty();

            RuleFor(r => r.Amount)
                .NotEmpty();

            RuleFor(r => r.Card)
                .NotNull();

            RuleFor(r => r.Card.CardNumber)
                .NotEmpty();

            RuleFor(r => r.Card.ExpirationMonth)
                .NotEmpty();

            RuleFor(r => r.Card.ExpirationYear)
                .NotEmpty();

            RuleFor(r => r.Card.SecurityCode)
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
            var order = await _dbContext.Orders
                .WithSpecification(new OrderByIdSpec(new OrderId(request.OrderId)))
                .FirstOrDefaultAsync(cancellationToken);

            if (order is null)
                return Error.NotFound();

            var amount = new Money(request.Amount);
            order.AddPayment(amount);

            _dbContext.Orders.Add(order);



            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response(order.Id.Value);
        }
    }
}