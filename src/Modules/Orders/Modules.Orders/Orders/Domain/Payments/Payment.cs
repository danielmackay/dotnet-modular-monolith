using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Orders.Orders.Domain.Payments;

internal record PaymentId(Guid Value) : IStronglyTypedId<Guid>
{
    internal PaymentId() : this(Guid.NewGuid())
    {
    }
}

// NOTE: Payment COULD have been an aggregate, but it's been implemented as an entity as we want to
// create a payment and apply it to an order in a single transaction. If it was an aggregate, it would
// would break the 'one aggregate per transaction' rule.
internal class Payment : Entity<PaymentId>
{
    public Money Amount { get; private set; } = null!;

    public PaymentType PaymentType { get; private set; } = null!;

    // Needed for EF
    private Payment()
    {
    }

    protected Payment(Money amount, PaymentType paymentType)
    {
        ArgumentNullException.ThrowIfNull(amount);
        ArgumentNullException.ThrowIfNull(paymentType);

        Id = new PaymentId(Uuid.Create());
        Amount = amount;
        PaymentType = paymentType;
    }
}

internal class CreditCardPayment : Payment
{
    public CreditCard Card { get; private set; }

    private CreditCardPayment(Money amount, CreditCard card) : base(amount, PaymentType.CreditCard)
    {
        Card = card;
    }

    internal static CreditCardPayment Create(Money amount, CreditCard card) => new(amount, card)
    {
        Id = new PaymentId(Uuid.Create())
    };
}

internal class CashPayment : Payment
{
    private CashPayment(Money money) : base(money, PaymentType.Cash)
    {
    }

    internal static CashPayment Create(Money amount) => new(amount)
    {
        Id = new PaymentId(Uuid.Create())
    };
}