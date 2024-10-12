using ErrorOr;

namespace Modules.Orders.Orders.Domain.Order;

public static class OrderErrors
{
    public static readonly Error CantModifyAfterPayment = Error.Validation(
        "Order.CantModifyAfterPayment",
        "Order can't be modified after payment");

    public static readonly Error CurrencyMismatch = Error.Validation(
        "Order.CurrencyMismatch",
        "Cannot add line item with different currency to an order");

    public static readonly Error PaymentAmountZeroOrNegative = Error.Validation(
        "Order.PaymentAmountZeroOrNegative",
        "Payment amount must be greater than zero");

    public static readonly Error PaymentExceedsOrderTotal = Error.Validation(
        "Order.PaymentExceedsOrderTotal",
        "Payment can't exceed order total");

    public static readonly Error CantShipUnpaidOrder = Error.Validation(
        "Order.CantShipUnpaidOrder",
        "Can't ship an unpaid order");

    public static readonly Error OrderAlreadyShipped = Error.Validation(
        "Order.OrderAlreadyShipped",
        "Order already shipped to customer");

    public static readonly Error OrderEmpty = Error.Validation(
        "Order.OrderEmpty",
        "Can't ship an order with no items");
}