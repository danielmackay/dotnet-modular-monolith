using Microsoft.AspNetCore.Routing;

namespace Common.SharedKernel.Discovery;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}