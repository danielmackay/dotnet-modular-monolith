using Common.SharedKernel.Domain.Ids;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Categories;
using Modules.Catalog.Products.UseCases;
using Modules.Customers.Customers.UseCases;
using Modules.Orders.Carts;
using Modules.Orders.Orders;
using Modules.Warehouse.BackOrders.Domain;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Products.UseCases;
using Modules.Warehouse.Storage.UseCases;
using System.Net.Http.Json;
using WebApi.Tests.Common;
using Xunit.Abstractions;

namespace WebApi.Tests;

/// <summary>
/// End-to-end process testing.
/// Purpose: These tests validate that a specific workflow (often a series of steps or a process) within a system behaves as expected when it runs from start to finish.
///	Scope: Workflow tests cover multiple components and interactions between them, simulating a user’s entire process, often from input to the final output or result.
/// </summary>
public class WorkflowTests(WorkflowDatabaseFixture fixture, ITestOutputHelper output)
    : WorkflowIntegrationTestBase(fixture, output)
{
    [Fact]
    public async Task Test()
    {
        var client = GetAnonymousClient();

        // Warehouse - Create Storage
        var createStorageReq = new CreateAisleCommand.Request("TestAisle", 5, 5);
        var response = await client.PostAsJsonAsync("api/aisles", createStorageReq);
        response.EnsureSuccessStatusCode();

        // Warehouse - Create Product
        var createProductReq = new CreateProductCommand.Request("TestProduct", "12345678");
        var response2 = await client.PostAsJsonAsync("api/products", createProductReq);
        response2.EnsureSuccessStatusCode();
        var createProductResp = await response2.Content.ReadFromJsonAsync<CreateProductCommand.Response>();
        var productId = createProductResp!.ProductId;

        // Warehouse - Allocate Storage
        var allocateStorageReq = new AllocateStorageCommand.Request(productId, 12);
        response = await client.PostAsJsonAsync("api/aisles/allocate-storage", allocateStorageReq);
        response.EnsureSuccessStatusCode();

        // Catalog - Create Category
        var createCategoryReq = new CreateCategoryCommand.Request("TestCategory");
        response = await client.PostAsJsonAsync("api/categories", createCategoryReq);
        response.EnsureSuccessStatusCode();
        var createCategoryResp = await response.Content.ReadFromJsonAsync<CreateCategoryCommand.Response>();
        var categoryId = createCategoryResp!.CategoryId;

        // Catalog - Add Product Category
        response = await client.PostAsync($"api/products/{productId}/categories/{categoryId}", null);
        response.EnsureSuccessStatusCode();

        // Catalog - Update Price
        var updatePriceReq = new UpdateProductPriceCommand.Request(5);
        response = await client.PutAsJsonAsync($"api/products/{productId}/price", updatePriceReq);
        response.EnsureSuccessStatusCode();

        // Customer - Register
        var registerCustomerReq = new RegisterCustomerCommand.Request("firstName", "lastName", "firstname.lastname@gmail.com");
        response = await client.PostAsJsonAsync("api/customers", registerCustomerReq);
        response.EnsureSuccessStatusCode();
        var registerCustomerResp = await response.Content.ReadFromJsonAsync<RegisterCustomerCommand.Response>();
        var customerId = registerCustomerResp!.CustomerId;

        // Orders - Add Product to Cart
        var addProductToCartReq = new AddProductToCartCommand.Request(null, productId, 10);
        response = await client.PostAsJsonAsync("api/carts", addProductToCartReq);
        response.EnsureSuccessStatusCode();
        var addProductToCartResp = await response.Content.ReadFromJsonAsync<AddProductToCartCommand.Response>();
        var cartId = addProductToCartResp!.CartId;

        // Orders - Checkout Order
        var checkoutOrderReq = new CheckoutCartCommand.Request(cartId, customerId);
        response = await client.PostAsJsonAsync("api/carts/checkout", checkoutOrderReq);
        response.EnsureSuccessStatusCode();
        var checkoutOrderResp = await response.Content.ReadFromJsonAsync<CheckoutCartCommand.Response>();
        var orderId = checkoutOrderResp!.OrderId;

        // Orders - Add Payment
        var makePaymentReq = new AddPaymentCommand.Request(orderId, 55, null);
        response = await client.PostAsJsonAsync($"api/orders/{orderId}/payment", makePaymentReq);
        response.EnsureSuccessStatusCode();

        // Orders - Ship Order
        var shipOrderReq = new ShipOrderCommand.Request(orderId);
        response = await client.PostAsJsonAsync($"api/orders/{orderId}/ship", shipOrderReq);
        response.EnsureSuccessStatusCode();

        // Allow for events to be processed
        await Task.Delay(5000);

        // Warehouse - confirm stock updated
        var product = await Database.GetQueryable<Product>().FirstOrDefaultAsync(p => p.Id == new ProductId(productId));
        product.Should().NotBeNull();
        product!.StockOnHand.Should().Be(2);

        // Warehouse - confirm stock re-ordered
        var backOrder = await Database.GetQueryable<BackOrder>().FirstOrDefaultAsync(o => o.ProductId == new ProductId(productId));
        backOrder.Should().NotBeNull();
        backOrder!.QuantityOrdered.Should().Be(10);
        backOrder.QuantityReceived.Should().Be(0);
    }
}