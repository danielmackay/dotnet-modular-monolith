namespace Modules.Orders.Orders.Domain.Payments;

internal record PaymentId(Guid Value);

internal class Payment : Entity<PaymentId>
{
    public Money Amount { get; private set; } = null!;

    public PaymentType PaymentType { get; private set; } = null!;

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

    internal CreditCardPayment(Money amount, CreditCard card) : base(amount, PaymentType.CreditCard)
    {
        Card = card;
    }
}

internal class CashPayment : Payment
{
    private CashPayment(Money money) : base(money, PaymentType.Cash)
    {
    }

    internal static CashPayment Create(Money amount) => new CashPayment(amount)
    {
        Id = new PaymentId(Uuid.Create())
    };
}