using Common.SharedKernel.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Modules.Orders.Orders.Domain.Payments;

public record CreditCard : IValueObject
{
    public string CardNumber { get; }
    public string ExpirationMonth { get; }
    public string ExpirationYear { get; }
    public string SecurityCode { get; }

    public CreditCard(string cardNumber, string expirationMonth, string expirationYear, string securityCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cardNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(expirationMonth);
        ArgumentException.ThrowIfNullOrWhiteSpace(expirationYear);
        ArgumentException.ThrowIfNullOrWhiteSpace(securityCode);

        if (!Regex.IsMatch(cardNumber, @"^4[0-9]{12}(?:[0-9]{3})?$"))
            throw new ArgumentException("Credit card number is not valid");

        if (!Regex.IsMatch(expirationMonth, @"^(0[1-9]|1[0-2])$"))
            throw new ArgumentException("Credit card expiration month is not valid");

        if (!Regex.IsMatch(expirationYear, @"^20[0-9]{2}$"))
            throw new ArgumentException("Credit card expiration year is not valid");

        if (!Regex.IsMatch(securityCode, @"^[0-9]{3,4}$"))
            throw new ArgumentException("Credit card security code is not valid");

        CardNumber = cardNumber;
        ExpirationMonth = expirationMonth;
        ExpirationYear = expirationYear;
        SecurityCode = securityCode;
    }
}