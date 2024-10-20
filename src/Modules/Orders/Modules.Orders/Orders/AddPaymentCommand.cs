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
using Modules.Orders.Orders.Infrastructure;

namespace Modules.Orders.Orders;

public static class AddPaymentCommand
{
    public record Request([FromRoute] Guid OrderId, decimal Amount, CreditCardDto? Card) : IRequest<ErrorOr<Success>>;

    public record CreditCardDto(string CardNumber, string ExpirationMonth, string ExpirationYear, string SecurityCode);

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
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(r => r.Card)
                .ChildRules(card =>
                {
                    card.RuleFor(c => c!.CardNumber)
                        .NotEmpty()
                        .Length(16);

                    card.RuleFor(c => c!.ExpirationMonth)
                        .NotEmpty()
                        .Length(2);

                    card.RuleFor(c => c!.ExpirationYear)
                        .NotEmpty()
                        .Length(4);

                    card.RuleFor(c => c!.SecurityCode)
                        .NotEmpty()
                        .Length(3);
                })
                .When(r => r.Card is not null);
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Success>>
    {
        private readonly OrdersDbContext _dbContext;
        private readonly IPaymentService _paymentService;

        public Handler(OrdersDbContext dbContext, IPaymentService paymentService)
        {
            _dbContext = dbContext;
            _paymentService = paymentService;
        }

        public async Task<ErrorOr<Success>> Handle(Request request, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders
                .WithSpecification(new OrderByIdSpec(new OrderId(request.OrderId)))
                .FirstOrDefaultAsync(cancellationToken);

            if (order is null)
                return Error.NotFound();

            var amount = new Money(request.Amount);
            if (request.Card is null)
            {
                var result = order.AddCashPayment(amount);

                if (result.IsError)
                    return result;
            }
            else
            {
                var card = new CreditCard(request.Card.CardNumber, request.Card.ExpirationMonth,
                    request.Card.ExpirationYear, request.Card.SecurityCode);
                var result = order.AddCreditCardPayment(amount, card);

                if (result.IsError)
                    return result;

                _paymentService.MakeCreditCardPayment(amount, card);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}