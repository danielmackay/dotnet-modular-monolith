using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Warehouse.Features.Categories.Domain;

namespace Modules.Warehouse.Features.Categories.Persistence;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
              .HasConversion(categoryId => categoryId.Value, value => new CategoryId(value));

        builder.Property(p => p.Name)
            .HasMaxLength(50);
    }
}
