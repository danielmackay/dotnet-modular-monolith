using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Categories;
using Modules.Catalog.Categories.Domain;
using Modules.Catalog.Tests.Common;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Modules.Catalog.Tests.Categories;

public class CategoryIntegrationTests(CatalogDatabaseFixture fixture, ITestOutputHelper output)
    : CatalogIntegrationTestBase(fixture, output)
{
    [Fact]
    public async Task CreateCategory_ValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var client = GetAnonymousClient();

        var request = new CreateCategoryCommand.Request("Name");

        // Act
        var response = await client.PostAsJsonAsync("/api/categories", request);

        // TODO: Add more asserts for API response content
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = await Database.GetQueryable<Category>().ToListAsync();
        categories.Should().HaveCount(1);

        var category = categories.First();
        category.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        category.CreatedBy.Should().NotBeNullOrWhiteSpace();
        category.Name.Should().Be(request.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateCategory_InvalidRequest_ReturnsBadRequest(string? name)
    {
        // Arrange
        var client = GetAnonymousClient();
        var request = new CreateCategoryCommand.Request(name!);

        // Act
        var response = await client.PostAsJsonAsync("/api/categories", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var categories = await Database.GetQueryable<Category>().ToListAsync();
        categories.Should().HaveCount(0);
    }

    [Fact]
    public async Task CreateCategory_DuplicateRequest_ReturnsBadRequest()
    {
        // Arrange
        var client = GetAnonymousClient();
        var request = new CreateCategoryCommand.Request("Name");

        // Act
        var response = await client.PostAsJsonAsync("/api/categories", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var response2 = await client.PostAsJsonAsync("/api/categories", request);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}