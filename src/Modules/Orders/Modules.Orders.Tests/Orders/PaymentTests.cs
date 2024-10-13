using Modules.Orders.Orders.Domain.Payments;

namespace Modules.Orders.Tests.Orders;

public class PaymentTests
{
    [Fact]
    public void Create_ShouldInitializePaymentWithCorrectValues()
    {
        // Arrange
        var amount = new Money(Currency.Default, 100);

        // Act
        var payment = CashPayment.Create(amount);

        // Assert
        payment.Amount.Should().Be(amount);
        payment.PaymentType.Should().Be(PaymentType.Cash);
        payment.Id.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldGenerateUniquePaymentId()
    {
        // Arrange
        var amount = new Money(Currency.Default, 100);

        // Act
        var payment1 = CashPayment.Create(amount);
        var payment2 = CashPayment.Create(amount);

        // Assert
        payment1.Id.Should().NotBe(payment2.Id);
    }

    [Fact]
    public void Create_ShouldThrowException_WhenAmountIsNull()
    {
        // Arrange
        Action act = () => CashPayment.Create(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    // TODO: Add tests for Credit Card Payments
}