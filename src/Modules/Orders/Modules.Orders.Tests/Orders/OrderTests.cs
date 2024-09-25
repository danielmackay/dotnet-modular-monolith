using Modules.Orders.Common;
using Modules.Orders.Orders.Order;

namespace Modules.Orders.Tests.Orders
{
    public class OrderTests
    {
        [Fact]
        public void AddLineItem_ShouldAddNewItem_WhenProductDoesNotExist()
        {
            // Arrange
            var order = Order.Create(new CustomerId(Uuid.Create()));
            var productId = new ProductId();
            var price = new Money(Currency.USD, 100);
            var quantity = 1;

            // Act
            var result = order.AddLineItem(productId, price, quantity);

            // Assert
            result.IsError.Should().BeFalse();
            order.LineItems.Should().HaveCount(1);
            order.LineItems.First().ProductId.Should().Be(productId);
        }

        [Fact]
        public void AddLineItem_ShouldIncreaseQuantity_WhenProductExists()
        {
            // Arrange
            var order = Order.Create(new CustomerId(Uuid.Create()));
            var productId = new ProductId();
            var price = new Money(Currency.USD, 100);
            var quantity = 1;
            order.AddLineItem(productId, price, quantity);

            // Act
            var result = order.AddLineItem(productId, price, quantity);

            // Assert
            result.IsError.Should().BeFalse();
            order.LineItems.Should().HaveCount(1);
            order.LineItems.First().Quantity.Should().Be(2);
        }

        [Fact]
        public void RemoveLineItem_ShouldRemoveItem_WhenProductExists()
        {
            // Arrange
            var order = Order.Create(new CustomerId(Uuid.Create()));
            var productId = new ProductId();
            var price = new Money(Currency.USD, 100);
            var quantity = 1;
            order.AddLineItem(productId, price, quantity);

            // Act
            var result = order.RemoveLineItem(productId);

            // Assert
            result.IsError.Should().BeFalse();
            order.LineItems.Should().BeEmpty();
        }

        [Fact]
        public void AddPayment_ShouldUpdateAmountPaid_WhenPaymentIsValid()
        {
            // Arrange
            var order = Order.Create(new CustomerId(Uuid.Create()));
            var productId = new ProductId();
            var price = new Money(Currency.USD, 100);
            var quantity = 1;
            order.AddLineItem(productId, price, quantity);
            order.AddShipping(new Money(Currency.USD, 10));

            // Act
            var result = order.AddPayment(new Money(Currency.USD, 110));

            // Assert
            result.IsError.Should().BeFalse();
            order.AmountPaid.Amount.Should().Be(110);
            order.Status.Should().Be(OrderStatus.PaymentReceived);
        }

        [Fact]
        public void ShipOrder_ShouldUpdateStatusAndShippingDate_WhenOrderIsReadyForShipping()
        {
            // Arrange
            var order = Order.Create(new CustomerId(Uuid.Create()));
            var productId = new ProductId();
            var price = new Money(Currency.USD, 100);
            var quantity = 1;
            order.AddLineItem(productId, price, quantity);
            order.AddShipping(new Money(Currency.USD, 10));
            order.AddPayment(new Money(Currency.USD, 110));
            var timeProvider = TimeProvider.System;

            // Act
            var result = order.ShipOrder(timeProvider);

            // Assert
            result.IsError.Should().BeFalse();
            order.Status.Should().Be(OrderStatus.InTransit);
            order.ShippingDate.Should().BeCloseTo(timeProvider.GetUtcNow(), TimeSpan.FromSeconds(1));
        }
    }
}