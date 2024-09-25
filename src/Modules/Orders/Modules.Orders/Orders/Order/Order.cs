using Common.SharedKernel.Domain.Exceptions;
using ErrorOr;
using Modules.Orders.Common;
using Modules.Orders.Orders.LineItem;
using Success = ErrorOr.Success;

namespace Modules.Orders.Orders.Order;

/*
 * An order must be associated with a customer - DONE
 * The order total must always be correct - DONE
 * The order tax must always be correct - DONE
 * Shipping must be included in the total price - DONE
 * Payment must be completed for the order to be placed - DONE
 */

internal class Order : AggregateRoot<OrderId>
{
    // 10% tax rate
    private const decimal TaxRate = 0.1m;

    private readonly List<LineItem.LineItem> _lineItems = [];

    public IEnumerable<LineItem.LineItem> LineItems => _lineItems.AsReadOnly();

    public required CustomerId CustomerId { get; init; }

    public Money AmountPaid { get; private set; } = null!;

    // private readonly Payment.Payment _payment = null!;

    public OrderStatus Status { get; private set; } = null!;

    public DateTimeOffset ShippingDate { get; private set; }

    public Currency? OrderCurrency => _lineItems.FirstOrDefault()?.Price.Currency;

    /// <summary>
    /// Total of all line items (including quantities). Excludes tax and shipping.
    /// </summary>
    public Money OrderSubTotal { get; private set; } = null!;


    /// <summary>
    /// Shipping total.  Excludes tax.
    /// </summary>
    public Money ShippingTotal { get; private set; } = null!;

    /// <summary>
    /// Tax of the order. Calculated on the OrderSubTotal and ShippingTotal.
    /// </summary>
    public Money TaxTotal { get; private set; } = null!;

    /// <summary>
    /// OrderSubTotal + ShippingTotal + TaxTotal
    /// </summary>
    public Money OrderTotal => OrderSubTotal + ShippingTotal + TaxTotal;



    private Order()
    {
    }

    public static Order Create(CustomerId customerId)
    {
        var order = new Order
        {
            Id = new OrderId(Uuid.Create()),
            CustomerId = customerId,
            AmountPaid = Money.Zero,
            OrderSubTotal = Money.Zero,
            ShippingTotal = Money.Zero,
            TaxTotal = Money.Zero,
            Status = OrderStatus.New
        };

        order.AddDomainEvent(OrderCreatedEvent.Create(order));

        return order;
    }

    public ErrorOr<LineItem.LineItem> AddLineItem(ProductId productId, Money price, int quantity)
    {
        // TODO: Unit test
        if (Status == OrderStatus.PaymentReceived)
            return OrderErrors.CantModifyAfterPayment;

        // TODO: Unit test
        if (OrderCurrency != null && OrderCurrency != price.Currency)
            return OrderErrors.CurrencyMismatch;

        var existingLineItem = _lineItems.FirstOrDefault(li => li.ProductId == productId);
        if (existingLineItem != null)
        {
            existingLineItem.AddQuantity(quantity);
            return existingLineItem;
        }

        var lineItem = LineItem.LineItem.Create(Id, productId, price, quantity);
        AddDomainEvent(new LineItemCreatedEvent(lineItem.Id, lineItem.OrderId));
        _lineItems.Add(lineItem);
        UpdateOrderTotal();

        return lineItem;
    }

    public ErrorOr<Success> RemoveLineItem(ProductId productId)
    {
        if (Status == OrderStatus.PaymentReceived)
            return OrderErrors.CantModifyAfterPayment;

        _lineItems.RemoveAll(x => x.ProductId == productId);
        UpdateOrderTotal();

        return Result.Success;
    }

    public void AddShipping(Money shipping)
    {
        // TODO: Do we need to check an order status here?
        ShippingTotal = shipping;
    }

    public ErrorOr<Success> AddPayment(Money payment)
    {
        if (payment.Amount <= 0)
            return OrderErrors.PaymentAmountZeroOrNegative;

        // Compare raw amounts to avoid error with default currency (i.e. AUD on $0 amounts)
        if (payment.Amount > OrderTotal.Amount - AmountPaid.Amount)
            return OrderErrors.PaymentExceedsOrderTotal;

        // Ensure currency is set on first payment
        if (AmountPaid.Amount == 0)
            AmountPaid = payment;
        else
            AmountPaid += payment;

        Status = OrderStatus.PaymentReceived;

        if (AmountPaid >= OrderTotal)
        {
            Status = OrderStatus.ReadyForShipping;
            AddDomainEvent(new OrderReadyForShippingEvent(Id));
        }

        return Result.Success;
    }

    public ErrorOr<Success> ShipOrder(TimeProvider timeProvider)
    {
        if (Status == OrderStatus.New)
            return OrderErrors.CantShipUnpaidOrder;

        if (Status == OrderStatus.InTransit)
            return OrderErrors.OrderAlreadyShipped;

        if (_lineItems.Sum(li => li.Quantity) <= 0)
            throw new DomainException("Can't ship an order with no items");

        ShippingDate = timeProvider.GetUtcNow();
        Status = OrderStatus.InTransit;

        return Result.Success;
    }

    private void UpdateOrderTotal()
    {
        if (_lineItems.Count == 0)
        {
            OrderSubTotal = Money.Zero;
            return;
        }

        var amount = _lineItems.Sum(li => li.Price.Amount * li.Quantity);
        var currency = OrderCurrency!;

        OrderSubTotal = new Money(currency, amount);
        TaxTotal = new Money(currency, OrderSubTotal.Amount * TaxRate);
    }
}