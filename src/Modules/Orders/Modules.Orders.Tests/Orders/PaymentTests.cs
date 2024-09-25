using Modules.Orders.Orders.Payment;

namespace Modules.Orders.Tests.Orders;

public class PaymentTests
{
    [Fact]
    public void Create_ShouldInitializePaymentWithCorrectValues()
    {
        // Arrange
        var amount = new Money(Currency.Default, 100);
        var paymentType = PaymentType.CreditCard;

        // Act
        var payment = Payment.Create(amount, paymentType);

        // Assert
        payment.Amount.Should().Be(amount);
        payment.PaymentType.Should().Be(paymentType);
        payment.Id.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldGenerateUniquePaymentId()
    {
        // Arrange
        var amount = new Money(Currency.Default, 100);
        var paymentType = PaymentType.CreditCard;

        // Act
        var payment1 = Payment.Create(amount, paymentType);
        var payment2 = Payment.Create(amount, paymentType);

        // Assert
        payment1.Id.Should().NotBe(payment2.Id);
    }

    [Fact]
    public void Create_ShouldThrowException_WhenAmountIsNull()
    {
        // Arrange
        Action act = () => Payment.Create(null!, PaymentType.CreditCard);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenPaymentTypeIsNull()
    {
        // Arrange
        var amount = new Money(Currency.Default, 100);
        Action act = () => Payment.Create(amount, null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
