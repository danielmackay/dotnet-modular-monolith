using Ardalis.SmartEnum;

namespace Modules.Orders.Orders.Domain.Payments;

internal class PaymentType : SmartEnum<PaymentType>
{
    public static readonly PaymentType Cash = new(1, "Cash");
    public static readonly PaymentType CreditCard = new(2, "CreditCard");
    // public static readonly PaymentType PayPal = new(2, "PayPal");

    private PaymentType(int id, string name) : base(name, id)
    {
    }
}