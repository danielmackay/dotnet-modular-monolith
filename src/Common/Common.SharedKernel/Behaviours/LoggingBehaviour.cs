using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Modules.Warehouse.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("ModularMonolith Request: {Name} {@Request}",
            requestName, request);

        return Task.CompletedTask;
    }
}
