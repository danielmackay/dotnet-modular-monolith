using Common.SharedKernel.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Warehouse.BackOrders;

namespace Modules.Warehouse.Common.Persistence.Configuration;

internal class BackOrderConfiguration : IEntityTypeConfiguration<BackOrder>
{
    public void Configure(EntityTypeBuilder<BackOrder> builder)
    {
        builder.HasKey(m => m.Id);

        builder
            .Property(m => m.Id)
            .HasStronglyTypedId<BackOrderId, Guid>()
            .ValueGeneratedNever();

        builder.Property(m => m.ProductId)
            .HasStronglyTypedId<ProductId, Guid>();
    }
}