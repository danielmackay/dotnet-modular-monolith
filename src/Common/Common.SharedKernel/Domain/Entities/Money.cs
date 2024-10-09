using Throw;

namespace Common.SharedKernel.Domain.Entities;

public record Money(Currency Currency, decimal Amount)
{
    public static Money Create(decimal amount) => new(Currency.Default, amount);

    public Money(decimal amount) : this(Currency.Default, amount)
    {
    }

    public static Money Default => new(Currency.Default, 0);

    public static Money Zero => Default;

    public static Money operator +(Money left, Money right)
    {
        AssertValidCurrencies(left, right);
        return left with { Amount = left.Amount + right.Amount };
    }

    public static Money operator -(Money left, Money right)
    {
        AssertValidCurrencies(left, right);
        return left with { Amount = left.Amount - right.Amount };
    }

    public static bool operator <(Money left, Money right)
    {
        AssertValidCurrencies(left, right);
        return left.Amount < right.Amount;
    }

    public static bool operator <=(Money left, Money right)
    {
        AssertValidCurrencies(left, right);
        return left.Amount <= right.Amount;
    }

    public static bool operator >(Money left, Money right)
    {
        return left.Amount > right.Amount;
    }

    public static bool operator >=(Money left, Money right)
    {
        return left.Amount >= right.Amount;
    }

    public static Money operator *(Money left, Money right)
    {
        AssertValidCurrencies(left, right);
        return left with { Amount = left.Amount * right.Amount };
    }

    private static void AssertValidCurrencies(Money left, Money right) => left.Currency.Throw().IfNotEquals(right.Currency);
}
