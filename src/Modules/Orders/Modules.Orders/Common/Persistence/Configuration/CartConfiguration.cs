using Common.SharedKernel.Persistence;
using Common.SharedKernel.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Orders.Carts.Domain;

namespace Modules.Orders.Common.Persistence.Configuration;

internal class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasStronglyTypedId<CartId, Guid>()
            .ValueGeneratedNever();

        builder.ComplexProperty(m => m.TotalPrice, MoneyConfiguration.BuildAction);

        builder.HasMany(p => p.Items);

        // TODO: Try to get this working.  Perhaps try owned entity?
        // builder.ComplexProperty(p => p.Items);
    }
}
