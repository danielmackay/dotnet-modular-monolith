using ErrorOr;

namespace Module.Orders.Orders;

public static class OrderErrors
{
    public static Error CantModifyAfterPayment = Error.Validation(
        "Order.CantModifyAfterPayment",
        "Order can't be modified after payment");

    public static Error CurrencyMismatch = Error.Validation(
        "Order.CurrencyMismatch",
        "Cannot add line item with different currency to an order");

    public static Error PaymentAmountZeroOrNegative = Error.Validation(
        "Order.PaymentAmountZeroOrNegative",
        "Payment amount must be greater than zero");

    public static Error PaymentExceedsOrderTotal = Error.Validation(
        "Order.PaymentExceedsOrderTotal",
        "Payment can't exceed order total");

    public static Error CantShipUnpaidOrder = Error.Validation(
        "Order.CantShipUnpaidOrder",
        "Can't ship an unpaid order");

    public static Error OrderAlreadyShipped = Error.Validation(
        "Order.OrderAlreadyShipped",
        "Order already shipped to customer");
}
