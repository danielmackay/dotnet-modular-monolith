using Microsoft.EntityFrameworkCore;
using Modules.Orders.Carts.Domain;
using Modules.Orders.Orders.Domain.Orders;
using SmartEnum.EFCore;

namespace Modules.Orders.Common.Persistence;

public class OrdersDbContext : DbContext
{
    internal DbSet<Cart> Carts => Set<Cart>();
    internal DbSet<Order> Orders => Set<Order>();

    // Needs to be public for the Database project
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
        modelBuilder.ConfigureSmartEnum();
        base.OnModelCreating(modelBuilder);
    }
}