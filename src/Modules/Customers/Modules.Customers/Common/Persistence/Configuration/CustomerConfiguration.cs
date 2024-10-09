using Common.SharedKernel.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Customers.Customers.Domain;

namespace Modules.Customers.Common.Persistence.Configuration;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(m => m.Id);

        builder
            .Property(m => m.Id)
            .HasStronglyTypedId<CustomerId, Guid>()
            .ValueGeneratedNever();

        // Using Owned as ComplexTypes don't support nullable records
        builder.OwnsOne(m => m.Address);
    }
}
