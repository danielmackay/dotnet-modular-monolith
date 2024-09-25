namespace Modules.Orders.Orders.Payment;

internal record PaymentId(Guid Value);

internal class Payment : Entity<PaymentId>
{
    public Money Amount { get; private set; } = null!;

    public PaymentType PaymentType { get; private set; } = null!;

    private Payment()
    {
    }

    public static Payment Create(Money amount, PaymentType paymentType)
    {
        ArgumentNullException.ThrowIfNull(amount);
        ArgumentNullException.ThrowIfNull(paymentType);

        var payment = new Payment
        {
            Id = new PaymentId(Uuid.Create()),
            Amount = amount,
            PaymentType = paymentType
        };

        return payment;
    }
}