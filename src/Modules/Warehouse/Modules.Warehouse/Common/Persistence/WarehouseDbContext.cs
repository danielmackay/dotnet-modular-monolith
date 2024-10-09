using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;

namespace Modules.Warehouse.Common.Persistence;

// Needs to be public for tests
public class WarehouseDbContext : DbContext
{
    internal DbSet<Aisle> Aisles => Set<Aisle>();

    internal DbSet<Product> Products => Set<Product>();

    // Needs to be public for the Database project
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("warehouse");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
