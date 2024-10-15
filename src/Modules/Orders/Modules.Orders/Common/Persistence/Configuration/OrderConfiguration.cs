using Common.SharedKernel.Domain.Ids;
using Common.SharedKernel.Persistence;
using Common.SharedKernel.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Orders.Orders.Domain.Orders;
using Modules.Orders.Orders.Domain.Payments;

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

        builder.HasOne(p => p.Payment)
            .WithMany();

        // TODO: Try to get this working.  Perhaps try owned entity?
        // builder.ComplexProperty(p => p.LineItems);
    }
}

internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(p => p.Id)
            .HasStronglyTypedId<PaymentId, Guid>()
            .ValueGeneratedNever();

        builder.ComplexProperty(m => m.Amount, MoneyConfiguration.BuildAction);
    }
}