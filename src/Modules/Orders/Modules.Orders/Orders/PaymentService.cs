using Microsoft.Extensions.Logging;
using Modules.Orders.Orders.Domain.Payments;

namespace Modules.Orders.Orders;

/// <summary>
/// Payment Infrastructure Service
/// </summary>
internal class FakePaymentService : IPaymentService
{
    private readonly ILogger<FakePaymentService> _logger;

    public FakePaymentService(ILogger<FakePaymentService> logger)
    {
        _logger = logger;
    }

    public void MakeCreditCardPayment(Money payment, CreditCard card)
    {
        _logger.LogInformation("Making credit card payment - money: {Money}, card: {Card}", payment, card);
    }
}

internal interface IPaymentService
{
    void MakeCreditCardPayment(Money payment, CreditCard card);
}