using Common.SharedKernel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.Common.Persistence;

namespace Modules.Warehouse.Common.Middleware;

internal class EventualConsistencyMiddleware
{
    // TODO: Make the key specific to each module
    public const string DomainEventsKey = "DomainEventsKey";

    private readonly RequestDelegate _next;

    public EventualConsistencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // TODO: See if we can make this middleware generic
    // TODO: Possibly use IDbContextFactory to dynamically create the context
    public async Task InvokeAsync(HttpContext context, IPublisher publisher, WarehouseDbContext dbContext)
    {
        // var transaction = await dbContext.Database.BeginTransactionAsync();
        context.Response.OnCompleted(async () =>
        {
            try
            {
                if (context.Items.TryGetValue(DomainEventsKey, out var value) && value is Queue<IDomainEvent> domainEvents)
                {
                    var strategy = dbContext.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        while (domainEvents.TryDequeue(out var nextEvent))
                        {
                            await publisher.Publish(nextEvent);
                        }
                    });
                }
                // await transaction.CommitAsync();
            }
            catch (EventualConsistencyException)
            {
                // handle eventual consistency exception
            }
            finally
            {
                // await transaction.DisposeAsync();
            }
        });

        await _next(context);
    }
}
