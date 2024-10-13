using Common.SharedKernel.Domain.Ids;
using Common.SharedKernel.Persistence;
using Common.SharedKernel.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Orders.Orders.Domain.Orders;

namespace Modules.Orders.Common.Persistence.Configuration;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasStronglyTypedId<OrderId, Guid>()
            .ValueGeneratedNever();

        builder.Property(p => p.CustomerId)
            .HasStronglyTypedId<CustomerId, Guid>()
            .ValueGeneratedNever();

        builder.ComplexProperty(m => m.AmountPaid, MoneyConfiguration.BuildAction);
        // builder.ComplexProperty(m => m.OrderTotal, MoneyConfiguration.BuildAction);
        builder.ComplexProperty(m => m.ShippingTotal, MoneyConfiguration.BuildAction);
        builder.ComplexProperty(m => m.TaxTotal, MoneyConfiguration.BuildAction);
        builder.ComplexProperty(m => m.OrderSubTotal, MoneyConfiguration.BuildAction);

        builder.HasMany(p => p.LineItems);

        // TODO: Try to get this working.  Perhaps try owned entity?
        // builder.ComplexProperty(p => p.LineItems);
    }
}