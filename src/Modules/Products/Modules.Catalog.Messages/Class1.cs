using ErrorOr;
using MediatR;

namespace Modules.Catalog.Messages;

public static class GetProductQuery
{
    public record Request(Guid ProductId) : IRequest<ErrorOr<Response>>;

    public record Response(string Name, Guid Id, string Sku, decimal Price, List<CategoryDto> Categories);

    public record CategoryDto(Guid Id, string Name);
}
