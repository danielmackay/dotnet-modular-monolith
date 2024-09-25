using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;
using Modules.Warehouse.Storage.UseCases;
using Modules.Warehouse.Tests.Common;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Modules.Warehouse.Tests.Storage.UseCases;

public class GetItemLocationQueryIntegrationTests(WarehouseDatabaseFixture fixture, ITestOutputHelper output)
    : WarehouseIntegrationTestBase(fixture, output)
{
    [Fact]
    public async Task Query_ValidRequest_ReturnsOk()
    {
        // Arrange
        var client = GetAnonymousClient();
        var product = Product.Create("Name", Sku.Create("12345678"));
        await AddEntityAsync(product);
        var aisle = Aisle.Create("Name", 2, 2);
        await AddEntityAsync(aisle);
        StorageAllocationService.AllocateStorage([aisle], product.Id);
        await SaveAsync();
        // var request = new GetItemLocationQuery.Request(product.Id.Value);

        // Act
        var response = await client.GetFromJsonAsync<GetItemLocationQuery.Response>($"/api/aisles/products/{product.Id.Value}");
        // var response = await client.GetAsync($"/api/aisles/products/{product.Id.Value}");
        // var content = response.Content.ReadAsStringAsync();

        // Assert
        response.Should().NotBeNull();
        response!.AisleName.Should().Be(aisle.Name);
        response.BayName.Should().Be("Bay 1");
        response.ShelfName.Should().Be("Shelf 1");
    }
}