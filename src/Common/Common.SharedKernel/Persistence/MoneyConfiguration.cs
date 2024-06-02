using Common.SharedKernel.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.SharedKernel.Persistence;

public static class MoneyConfiguration
{
    public static void BuildAction(ComplexPropertyBuilder<Money> priceBuilder)
    {
        priceBuilder.Property(m => m.Currency)
            .HasConversion(currency => currency.Symbol, value => new Currency(value))
            .HasMaxLength(3);

        priceBuilder.Property(m => m.Amount).HasPrecision(18, 2);
    }
}
