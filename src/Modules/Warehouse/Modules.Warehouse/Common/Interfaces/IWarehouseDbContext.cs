using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.Features.Categories.Domain;
using Modules.Warehouse.Features.Products.Domain;

namespace Modules.Warehouse.Common.Interfaces;

// TODO: Consider removing
public interface IWarehouseDbContext
{
    public DbSet<Category> Categories { get; }
    
    public DbSet<Product> Products { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
