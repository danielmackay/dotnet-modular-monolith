using Common.SharedKernel;
using Common.SharedKernel.Api;
using Common.SharedKernel.Discovery;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Catalog.Categories.Domain;
using Modules.Catalog.Common.Persistence;

namespace Modules.Catalog.Categories;

public static class CreateCategoryCommand
{
    public record Request(string Name) : IRequest<ErrorOr<Response>>;

    public record Response(Guid CategoryId);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/categories", async (Request request, ISender sender) =>
                {
                    var response = await sender.Send(request);
                    return response.Match(TypedResults.Ok, ErrorOrExt.Problem);
                })
                .WithName("CreateCategory")
                .WithTags("Catalog")
                .ProducesPost()
                .WithOpenApi();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Response>>
    {
        private readonly CatalogDbContext _dbContext;

        public Handler(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var exists = _dbContext.Categories.Any(c => c.Name == request.Name);

            if (exists)
                return CategoryErrors.DuplicateName;

            var category = Category.Create(request.Name);
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response(category.Id.Value);
        }
    }
}