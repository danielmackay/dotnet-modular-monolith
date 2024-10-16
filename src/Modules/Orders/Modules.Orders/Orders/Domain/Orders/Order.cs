﻿using Common.SharedKernel.Domain.Ids;
using ErrorOr;
using Modules.Orders.Orders.Domain.LineItems;
using Modules.Orders.Orders.Domain.Payments;

namespace Modules.Orders.Orders.Domain.Orders;

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

    private readonly List<LineItem> _lineItems = [];

    public IEnumerable<LineItem> LineItems => _lineItems.AsReadOnly();

    public required CustomerId CustomerId { get; init; }

    public Money AmountPaid { get; private set; } = null!;

    // TODO: Allow multiple payments
    public Payment? Payment { get; private set; }

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

        order.AddDomainEvent(new OrderCreatedEvent(order));

        return order;
    }

    public ErrorOr<LineItem> AddLineItem(ProductId productId, Money price, int quantity)
    {
        // TODO: Unit test
        if (Status == OrderStatus.Paid)
            return OrderErrors.CantModifyAfterPayment;

        if (Status == OrderStatus.Shipped)
            return Error.Conflict("Can't add an item once it's shipped");

        if (Status == OrderStatus.Delivered)
            return Error.Conflict("Can't add an item once it's delivered");

        // TODO: Unit test
        if (OrderCurrency != null && OrderCurrency != price.Currency)
            return OrderErrors.CurrencyMismatch;

        var existingLineItem = _lineItems.FirstOrDefault(li => li.ProductId == productId);
        if (existingLineItem != null)
        {
            existingLineItem.AddQuantity(quantity);
            return existingLineItem;
        }

        var lineItem = LineItem.Create(Id, productId, price, quantity);
        AddDomainEvent(new LineItemCreatedEvent(lineItem.Id, lineItem.OrderId));
        _lineItems.Add(lineItem);
        UpdateOrderTotal();

        return lineItem;
    }

    public ErrorOr<Success> RemoveLineItem(ProductId productId)
    {
        if (Status == OrderStatus.Paid)
            return OrderErrors.CantModifyAfterPayment;

        if (Status == OrderStatus.Shipped)
            return Error.Conflict("Can't remove an item once it's shipped");

        if (Status == OrderStatus.Delivered)
            return Error.Conflict("Can't remove an item once it's delivered");

        _lineItems.RemoveAll(x => x.ProductId == productId);
        UpdateOrderTotal();

        return Result.Success;
    }

    public void AddShipping(Money shipping)
    {
        // TODO: Do we need to check an order status here?
        ShippingTotal = shipping;
    }


    public ErrorOr<Success> AddCreditCardPayment(Money payment, CreditCard card)
    {
        Payment = CreditCardPayment.Create(payment, card);
        return AddPayment(payment);
    }

    public ErrorOr<Success> AddCashPayment(Money payment)
    {
        Payment = CashPayment.Create(payment);
        return AddPayment(payment);
    }

    private ErrorOr<Success> AddPayment(Money payment)
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

        if (AmountPaid >= OrderTotal)
        {
            Status = OrderStatus.Paid;
            AddDomainEvent(new OrderPaidEvent(this));
        }

        return Result.Success;
    }

    public ErrorOr<Success> ShipOrder(TimeProvider timeProvider)
    {
        if (Status == OrderStatus.New)
            return OrderErrors.CantShipUnpaidOrder;

        if (Status == OrderStatus.Shipped)
            return OrderErrors.OrderAlreadyShipped;

        if (_lineItems.Sum(li => li.Quantity) <= 0)
            return OrderErrors.OrderEmpty;

        ShippingDate = timeProvider.GetUtcNow();
        Status = OrderStatus.Shipped;

        AddDomainEvent(new OrderShippedEvent(this));

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

    public IEnumerable<ProductId> GetProducts() => LineItems.Select(li => li.ProductId).ToList();
}