using Common.Tests.Assertions;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Products.Domain;
using Modules.Orders.Carts;
using Modules.Orders.Carts.Domain;
using Modules.Orders.Tests.Common;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Modules.Orders.Tests.Carts;

public class CartIntegrationTests(OrdersDatabaseFixture fixture, ITestOutputHelper output)
    : OrdersIntegrationTestBase(fixture, output)
{
    [Fact]
    public async Task CreateCart_WithValidRequest_ReturnsCart()
    {
        // Arrange
        var product = Product.Create("name", "12345678");
        product.UpdatePrice(new Money(100));
        fixture.CatalogDbContext.Products.Add(product);
        await fixture.CatalogDbContext.SaveChangesAsync();
        var client = GetAnonymousClient();
        var quantity = 1;
        var request = new AddProductToCartCommand.Request(null, product.Id.Value, quantity);

        // Act
        var response = await client.PostAsJsonAsync("/api/carts", request);

        // Assert
        HttpContentExtensions.Should(response).BeStatusCode(HttpStatusCode.OK);
        var carts = await GetQueryable<Cart>().Include(c => c.Items).ToListAsync();
        carts.Should().HaveCount(1);

        var cart = carts.First();
        cart.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        cart.CreatedBy.Should().NotBeNullOrWhiteSpace();
        cart.Id.Should().NotBeNull();
        cart.Items.Should().HaveCount(1);

        var item = cart.Items.First();
        item.Should().NotBeNull();
        item.ProductId.Should().Be(product.Id);
        item.Quantity.Should().Be(quantity);
        item.LinePrice.Amount.Should().Be(100);
        item.UnitPrice.Amount.Should().Be(100);
    }

    [Fact]
    public async Task AddProduct_WithExistingCart_ReturnsCart()
    {
        // Arrange
        var product = Product.Create("name", "12345678");
        product.UpdatePrice(new Money(100));
        fixture.CatalogDbContext.Products.Add(product);
        await fixture.CatalogDbContext.SaveChangesAsync();
        var client = GetAnonymousClient();
        var quantity = 1;
        var request1 = new AddProductToCartCommand.Request(null, product.Id.Value, quantity);
        var response1 = await client.PostAsJsonAsync("/api/carts", request1);
        var content = await response1.Content.ReadFromJsonAsync<AddProductToCartCommand.Response>();
        var request2 = new AddProductToCartCommand.Request(content!.CartId, product.Id.Value, quantity);

        // Act
        var response2 = await client.PostAsJsonAsync("/api/carts", request2);

        // Assert
        HttpContentExtensions.Should(response2).BeStatusCode(HttpStatusCode.OK);
        var carts = await GetQueryable<Cart>().Include(c => c.Items).ToListAsync();
        carts.Should().HaveCount(1);

        var cart = carts.First();
        cart.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        cart.CreatedBy.Should().NotBeNullOrWhiteSpace();
        cart.Id.Should().NotBeNull();
        cart.Items.Should().HaveCount(1);

        var item = cart.Items.First();
        item.Should().NotBeNull();
        item.ProductId.Should().Be(product.Id);
        item.Quantity.Should().Be(quantity + quantity);
        item.LinePrice.Amount.Should().Be(200);
        item.UnitPrice.Amount.Should().Be(100);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", 1)]
    [InlineData("73060DE4-5AD8-4574-B857-5D5C5F44203F", 0)]
    [InlineData("73060DE4-5AD8-4574-B857-5D5C5F44203F", -1)]
    public async Task CreateProduct_InvalidRequest_ReturnsBadRequest(string productId, int quantity)
    {
        // Arrange
        var client = GetAnonymousClient();
        var request = new AddProductToCartCommand.Request(null, Guid.Parse(productId), quantity);

        // Act
        var response = await client.PostAsJsonAsync("/api/carts", request);

        // Assert
        HttpContentExtensions.Should(response).BeStatusCode(HttpStatusCode.BadRequest);
    }
}
