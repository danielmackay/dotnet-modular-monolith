using Common.SharedKernel;
using Common.SharedKernel.Api;
using Common.SharedKernel.Discovery;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Customers.Common.Persistence;
using Modules.Customers.Customers.Domain;

namespace Modules.Customers.Customers.UseCases;

public static class RegisterCustomerCommand
{
    public record Request(string FirstName, string LastName, string Email) : IRequest<ErrorOr<Response>>;

    public record Response(Guid CustomerId);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/customers", async (Request request, ISender sender) =>
                {
                    var response = await sender.Send(request);
                    return response.Match(TypedResults.Ok, ErrorOrExt.Problem);
                })
                .WithName("Create Customer")
                .WithTags("Customers")
                .ProducesPost()
                .WithOpenApi();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.FirstName).NotEmpty();
            RuleFor(r => r.LastName).NotEmpty();
            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Response>>
    {
        private readonly CustomersDbContext _dbContext;

        public Handler(CustomersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var customer = Customer.Create(request.Email, request.FirstName, request.LastName);
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response(customer.Id.Value);
        }
    }
}