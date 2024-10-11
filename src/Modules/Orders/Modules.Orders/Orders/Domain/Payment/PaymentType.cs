using Ardalis.SmartEnum;

namespace Modules.Orders.Orders.Payment;

internal class PaymentType : SmartEnum<PaymentType>
{
    public static readonly PaymentType CreditCard = new(1, "CreditCard");
    public static readonly PaymentType PayPal = new(2, "PayPal");
    public static readonly PaymentType Cash = new(3, "Cash");

    private PaymentType(int id, string name) : base(name, id)
    {
    }
}
