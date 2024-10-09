using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Modules.Customers.Customers.Domain;

namespace Modules.Customers.Common.Persistence;

// Needs to be public for tests
public class CustomersDbContext : DbContext
{
    internal DbSet<Customer> Customers => Set<Customer>();


    // Needs to be public for the Database project
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("customer");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Produces easy to read exceptions
        optionsBuilder.UseExceptionProcessor();

        base.OnConfiguring(optionsBuilder);
    }
}
