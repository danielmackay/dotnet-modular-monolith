using Common.Tests.Assertions;
using Microsoft.EntityFrameworkCore;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Products.UseCases;
using Modules.Warehouse.Tests.Common;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Modules.Warehouse.Tests.Products;

public class ProductIntegrationTests(WarehouseDatabaseFixture fixture, ITestOutputHelper output)
    : WarehouseIntegrationTestBase(fixture, output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task CreateProduct_ValidRequest_ReturnsCreatedProduct()
    {
        // Arrange
        var client = GetAnonymousClient();
        var request = new CreateProductCommand.Request("Name", "12345678");

        // Act
        var response = await client.PostAsJsonAsync("/api/products", request);

        // Assert
        HttpContentExtensions.Should(response).BeStatusCode(HttpStatusCode.Created);
        var products = await GetQueryable<Product>().ToListAsync();
        products.Should().HaveCount(1);

        var product = products.First();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        product.CreatedBy.Should().NotBeNullOrWhiteSpace();
        product.Name.Should().Be(request.Name);
        product.Sku.Value.Should().Be(request.Sku);
    }

    [Theory]
    [InlineData(null, "12345678")]
    [InlineData("", "12345678")]
    [InlineData(" ", "12345678")]
    [InlineData("name", null)]
    [InlineData("name", "")]
    [InlineData("name", "123")]
    public async Task CreateProduct_InvalidRequest_ReturnsBadRequest(string? name, string? sku)
    {
        // Arrange
        var client = GetAnonymousClient();
        var request = new CreateProductCommand.Request(name!, sku!);

        // Act
        var response = await client.PostAsJsonAsync("/api/products", request);

        // Assert
        HttpContentExtensions.Should(response).BeStatusCode(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine(content);
    }
}
