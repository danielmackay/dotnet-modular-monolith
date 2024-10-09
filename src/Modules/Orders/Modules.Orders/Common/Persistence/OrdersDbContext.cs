using Microsoft.EntityFrameworkCore;
using Modules.Orders.Carts.Domain;

namespace Modules.Orders.Common.Persistence;

public class OrdersDbContext : DbContext
{
    internal DbSet<Cart> Carts => Set<Cart>();

    // Needs to be public for the Database project
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
