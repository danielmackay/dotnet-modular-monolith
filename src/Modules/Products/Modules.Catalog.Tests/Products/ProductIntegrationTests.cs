using Ardalis.Specification.EntityFrameworkCore;
using Common.SharedKernel.Domain;
using FluentAssertions;
using Modules.Catalog.Categories.Domain;
using Modules.Catalog.Messages;
using Modules.Catalog.Products.Domain;
using Modules.Catalog.Products.UseCases;
using Modules.Catalog.Tests.Common;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Modules.Catalog.Tests.Products;

public class ProductIntegrationTests(CatalogDatabaseFixture fixture, ITestOutputHelper output)
    : CatalogIntegrationTestBase(fixture, output)
{
    [Fact]
    public async Task AddProductCategory_ValidRequest_ShouldReturnNotContent()
    {
        // Arrange
        var client = GetAnonymousClient();
        var category = Category.Create("category");
        await AddEntityAsync(category);
        var product = Product.Create("product", "12345678");
        await AddEntityAsync(product);

        // Act
        var response = await client.PostAsync($"/api/products/{product.Id.Value}/categories/{category.Id.Value}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var updatedProduct = GetQueryable<Product>()
            .WithSpecification(new ProductByIdSpec(new ProductId(product.Id.Value))).FirstOrDefault();
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Categories.Should().HaveCount(1);
        updatedProduct.Categories[0].Should().BeEquivalentTo(category);
    }

    [Fact]
    public async Task RemoveProductCategory_ValidRequest_ShouldReturnNotContent()
    {
        // Arrange
        var client = GetAnonymousClient();
        var category = Category.Create("category");
        await AddEntityAsync(category);
        var product = Product.Create("product", "12345678");
        await AddEntityAsync(product);
        product.AddCategory(category);
        await SaveAsync();

        // Act
        var response = await client.DeleteAsync($"/api/products/{product.Id.Value}/categories/{category.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var updatedProduct = GetQueryable<Product>()
            .WithSpecification(new ProductByIdSpec(new ProductId(product.Id.Value))).FirstOrDefault();
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Categories.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetProductQuery_WhenProductExists_ShouldReturnOk()
    {
        // Arrange
        var client = GetAnonymousClient();
        var category = Category.Create("category");
        await AddEntityAsync(category);
        var product = Product.Create("product", "12345678");
        await AddEntityAsync(product);
        product.AddCategory(category);
        await SaveAsync();

        // Act
        var response = await client.GetAsync($"/api/products/{product.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<GetProductQuery.Response>();
        json.Should().NotBeNull();
        json!.Name.Should().Be(product.Name);
        json.Sku.Should().Be(product.Sku);
        json.Price.Should().Be(product.Price.Amount);
        json.Categories.Should().HaveCount(1);
        json.Categories[0].Name.Should().Be(category.Name);
    }

    [Fact]
    public async Task GetProductQuery_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = GetAnonymousClient();

        // Act
        var response = await client.GetAsync($"/api/products/{Uuid.Create()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProductPrice_ValidRequest_ShouldReturnNoContent()
    {
        // Arrange
        var client = GetAnonymousClient();
        var product = Product.Create("product", "12345678");
        await AddEntityAsync(product);
        await SaveAsync();
        var request = new UpdateProductPriceCommand.Request(10.0m);

        // Act
        var response = await client.PutAsJsonAsync($"/api/products/{product.Id.Value}/price", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var updatedProduct = GetQueryable<Product>()
            .WithSpecification(new ProductByIdSpec(new ProductId(product.Id.Value))).FirstOrDefault();
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Price.Amount.Should().Be(request.Price);
    }
}
