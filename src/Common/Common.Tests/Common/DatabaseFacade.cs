using Microsoft.EntityFrameworkCore;

namespace Common.Tests.Common;

public class DatabaseFacade<TDbContext> where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public DatabaseFacade(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class => _dbContext.Set<TEntity>().AsNoTracking();

    public async Task AddEntityAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
    {
        await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddEntitiesAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
    {
        await _dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}