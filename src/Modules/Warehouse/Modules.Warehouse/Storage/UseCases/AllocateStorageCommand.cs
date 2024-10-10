using Common.SharedKernel.Api;
using Common.SharedKernel.Discovery;
using ErrorOr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;

namespace Modules.Warehouse.Storage.UseCases;

public static class AllocateStorageCommand
{
    public record Request(Guid ProductId) : IRequest<ErrorOr<Success>>;

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/aisles/allocate-storage", async (Request request, ISender sender) =>
                {
                    var response = await sender.Send(request);
                    return response.IsError ? response.Problem() : TypedResults.Ok();
                })
                .WithName("Allocate Storage")
                .WithTags("Warehouse")
                .WithOpenApi();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.ProductId).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, ErrorOr<Success>>
    {
        private readonly WarehouseDbContext _dbContext;

        public Handler(WarehouseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<Success>> Handle(Request request, CancellationToken cancellationToken)
        {
            var aisles = await _dbContext.Aisles
                .WithSpecification(new GetAllAislesSpec())
                .ToListAsync(cancellationToken);

            var product = await _dbContext.Products
                .WithSpecification(new ProductByIdSpec(new ProductId(request.ProductId)))
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
                return Error.NotFound("Product not found");

            var result = StorageAllocationService.AllocateStorage(aisles, product.Id);
            if (result.IsError)
                return result.FirstError;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }
    }
}