using Common.SharedKernel.Domain.Ids;
using Common.SharedKernel.Persistence;
using Common.SharedKernel.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Orders.Orders.Domain.LineItem;

namespace Modules.Orders.Common.Persistence.Configuration;

internal class LineItemConfiguration : IEntityTypeConfiguration<LineItem>
{
    public void Configure(EntityTypeBuilder<LineItem> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasStronglyTypedId<LineItemId, Guid>()
            .ValueGeneratedNever();

        builder.Property(p => p.ProductId)
            .HasStronglyTypedId<ProductId, Guid>()
            .ValueGeneratedNever();

        builder.ComplexProperty(m => m.Price, MoneyConfiguration.BuildAction);
        builder.ComplexProperty(m => m.Tax, MoneyConfiguration.BuildAction);
        builder.ComplexProperty(m => m.Total, MoneyConfiguration.BuildAction);
        builder.ComplexProperty(m => m.TotalIncludingTax, MoneyConfiguration.BuildAction);
    }
}