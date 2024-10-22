using Common.Tests.Assertions;
using Microsoft.EntityFrameworkCore;
using Modules.Orders.Orders;
using Modules.Orders.Orders.Domain.Orders;
using Modules.Orders.Orders.Domain.Payments;
using Modules.Orders.Tests.Common;
using Modules.Orders.Tests.Common.Builders;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Modules.Orders.Tests.Orders;

public class OrderIntegrationTests(OrdersDatabaseFixture fixture, ITestOutputHelper output)
    : OrdersIntegrationTestBase(fixture, output)
{
    [Fact]
    public async Task AddCreditCardPayment_WithValidOrder_Succeeds()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithLineItem()
            .Build();
        await OrdersDb.AddEntityAsync(order);
        var card = NewCreditCard();
        var client = GetAnonymousClient();
        var paymentAmount = order.OrderTotal.Amount;
        var request = new AddPaymentCommand.Request(order.Id.Value, paymentAmount, card);

        // Act
        var response = await client.PostAsJsonAsync($"api/orders/{order.Id.Value}/payment", request);

        // Assert
        response.ShouldHave().StatusCode(HttpStatusCode.OK);
        var paidOrder = await OrdersDb.GetQueryable<Order>()
            .Include(i => i.LineItems)
            .Include(i => i.Payment)
            .FirstOrDefaultAsync();

        paidOrder.Should().NotBeNull();
        paidOrder!.AmountPaid.Amount.Should().Be(paymentAmount);
        paidOrder.Payment.Should().NotBeNull();
        paidOrder.Payment!.Amount.Amount.Should().Be(paymentAmount);
        paidOrder.Payment.PaymentType.Should().Be(PaymentType.CreditCard);
    }

    [Fact]
    public async Task AddCashPayment_WithValidOrder_Succeeds()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithLineItem()
            .Build();
        await OrdersDb.AddEntityAsync(order);
        var client = GetAnonymousClient();
        var paymentAmount = order.OrderTotal.Amount;
        var request = new AddPaymentCommand.Request(order.Id.Value, paymentAmount, null);

        // Act
        var response = await client.PostAsJsonAsync($"api/orders/{order.Id.Value}/payment", request);

        // Assert
        response.ShouldHave().StatusCode(HttpStatusCode.OK);
        var paidOrder = await OrdersDb.GetQueryable<Order>()
            .Include(i => i.LineItems)
            .Include(i => i.Payment)
            .FirstOrDefaultAsync();

        paidOrder.Should().NotBeNull();
        paidOrder!.AmountPaid.Amount.Should().Be(paymentAmount);
        paidOrder.Payment.Should().NotBeNull();
        paidOrder.Payment!.Amount.Amount.Should().Be(paymentAmount);
        paidOrder.Payment.PaymentType.Should().Be(PaymentType.Cash);
    }

    [Fact]
    public async Task AddCashPayment_WithNotFoundOrder_ReturnsNotFound()
    {
        // Arrange
        var client = GetAnonymousClient();
        var paymentAmount = 100;
        var orderId = Uuid.Create();
        var request = new AddPaymentCommand.Request(orderId, paymentAmount, null);

        // Act
        var response = await client.PostAsJsonAsync($"api/orders/{orderId}/payment", request);

        // Assert
        response.ShouldHave().StatusCode(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(-1, "4444333322221111", "01", "2024", "123")]
    [InlineData(0, "4444333322221111", "01", "2024", "123")]
    [InlineData(100, "", "01", "2024", "123")]
    [InlineData(100, null, "01", "2024", "123")]
    [InlineData(100, "4444333322221111", "", "2024", "123")]
    [InlineData(100, "4444333322221111", null, "2024", "123")]
    [InlineData(100, "4444333322221111", "01", "", "123")]
    [InlineData(100, "4444333322221111", "01", null, "123")]
    [InlineData(100, "4444333322221111", "01", "2024", "")]
    [InlineData(100, "4444333322221111", "01", "2024", null)]
    public async Task AddCashPayment_WithInvalidOrder_ReturnsBadRequest(
        decimal paymentAmount,
        string cardNumber,
        string expMonth,
        string expYear,
        string code)
    {
        // Arrange
        var card = new AddPaymentCommand.CreditCardDto(cardNumber, expMonth, expYear, code);
        var request = new AddPaymentCommand.Request(Uuid.Create(), paymentAmount, card);
        var client = GetAnonymousClient();

        // Act
        var response = await client.PostAsJsonAsync($"api/orders/{request.OrderId}/payment", request);

        // Assert
        response.ShouldHave().StatusCode(HttpStatusCode.BadRequest);

    }

    // TODO: Add Order Shipped Command Integration Tests


    private static AddPaymentCommand.CreditCardDto NewCreditCard() => new("4444333322221111", "01", "2030", "123");
}