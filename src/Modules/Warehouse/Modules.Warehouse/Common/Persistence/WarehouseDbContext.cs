﻿using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.BackOrders.Domain;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;
using SmartEnum.EFCore;

namespace Modules.Warehouse.Common.Persistence;

// Needs to be public for tests
public class WarehouseDbContext : DbContext
{
    internal DbSet<Aisle> Aisles => Set<Aisle>();

    internal DbSet<Shelf> Shelves => Set<Shelf>();

    internal DbSet<Product> Products => Set<Product>();

    internal DbSet<BackOrder> BackOrders => Set<BackOrder>();

    // Needs to be public for the Database project
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("warehouse");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
        modelBuilder.ConfigureSmartEnum();
        base.OnModelCreating(modelBuilder);
    }
}