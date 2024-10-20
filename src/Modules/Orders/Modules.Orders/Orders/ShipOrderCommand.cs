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

public static class ShipOrderCommand
{
    public record Request([FromRoute] Guid OrderId) : IRequest<ErrorOr<Success>>;

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/orders/{orderId:guid}/ship",
                    async (Request request, ISender sender) =>
                    {
                        var response = await sender.Send(request);
                        return response.Match(TypedResults.Ok, ErrorOrExt.Problem);
                    })
                .WithName("ShipOrder")
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
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Success>>
    {
        private readonly OrdersDbContext _dbContext;
        private readonly TimeProvider _timeProvider;

        public Handler(OrdersDbContext dbContext, TimeProvider timeProvider)
        {
            _dbContext = dbContext;
            _timeProvider = timeProvider;
        }

        public async Task<ErrorOr<Success>> Handle(Request request, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders
                .WithSpecification(new OrderByIdSpec(new OrderId(request.OrderId)))
                .FirstOrDefaultAsync(cancellationToken);

            if (order is null)
                return Error.NotFound();

            var result = order.ShipOrder(_timeProvider);
            if (result.IsError)
                return result;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}