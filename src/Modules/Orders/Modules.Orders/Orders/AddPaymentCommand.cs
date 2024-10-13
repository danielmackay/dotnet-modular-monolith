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
using Modules.Orders.Orders.Domain.Payments;

namespace Modules.Orders.Orders;

public static class AddPaymentCommand
{
    // TODO: Test if FromRoute parameter is correct
    public record Request([FromRoute] Guid OrderId, decimal Amount, CreditCardDto? Card) : IRequest<ErrorOr<Response>>;

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

            // TODO: Fix up nested rules
            // RuleFor(r => r.Card)
            //     .ChildRules(rules => rules
            //         .RuleFor(r => r.CardNumber).NotEmpty()
            //         .RuleFor(r => r.CardNumber).NotEmpty()
            //     )
            //     .When(r => r.Card is not null);
            //
            // RuleFor(r => r.Card)
            //     .When(c => c.Card is not null)
            //     .NotNull();

            RuleFor(r => r.Card!.CardNumber)
                .NotEmpty()
                .When(r => r.Card is not null);

            RuleFor(r => r.Card!.ExpirationMonth)
                .NotEmpty()
                .When(r => r.Card is not null);

            RuleFor(r => r.Card!.ExpirationYear)
                .NotEmpty()
                .When(r => r.Card is not null);

            RuleFor(r => r.Card!.SecurityCode)
                .NotEmpty()
                .When(r => r.Card is not null);
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Response>>
    {
        private readonly OrdersDbContext _dbContext;
        private readonly IPaymentService _paymentService;

        public Handler(OrdersDbContext dbContext, IPaymentService paymentService)
        {
            _dbContext = dbContext;
            _paymentService = paymentService;
        }

        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders
                .WithSpecification(new OrderByIdSpec(new OrderId(request.OrderId)))
                .FirstOrDefaultAsync(cancellationToken);

            if (order is null)
                return Error.NotFound();

            var amount = new Money(request.Amount);
            if (request.Card is null)
            {
                order.AddCashPayment(amount);
            }
            else
            {
                var card = new CreditCard(request.Card.CardNumber, request.Card.ExpirationMonth,
                    request.Card.ExpirationYear, request.Card.SecurityCode);
                order.AddCreditCardPayment(amount, card);
                _paymentService.MakeCreditCardPayment(amount, card);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response();
        }
    }
}