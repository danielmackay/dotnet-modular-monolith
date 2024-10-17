using Common.SharedKernel.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;

namespace Modules.Warehouse.Common.Persistence.Configuration;

internal class ShelfConfiguration : IEntityTypeConfiguration<Shelf>
{
    public void Configure(EntityTypeBuilder<Shelf> builder)
    {
        builder.HasKey(m => m.Id);

        builder
            .Property(m => m.Id)
            .HasStronglyTypedId<ShelfId, Guid>()
            .ValueGeneratedNever();

        builder.Property(m => m.Name)
            .IsRequired();

        builder.Property(m => m.ProductId)!
            .HasStronglyTypedId<ProductId, Guid>()
            .IsRequired(false);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(m => m.ProductId);
    }
}