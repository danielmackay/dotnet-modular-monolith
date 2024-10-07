using Common.SharedKernel.Domain.Ids;
using Common.SharedKernel.Persistence;
using Common.SharedKernel.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Orders.Carts.Domain;

namespace Modules.Orders.Common.Persistence.Configuration;

internal class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasStronglyTypedId<CartItemId, Guid>()
            .ValueGeneratedNever();

        builder.Property(p => p.ProductId)
            .HasStronglyTypedId<ProductId, Guid>()
            .ValueGeneratedNever();

        builder.ComplexProperty(m => m.UnitPrice, MoneyConfiguration.BuildAction);
        builder.ComplexProperty(m => m.LinePrice, MoneyConfiguration.BuildAction);
    }
}
