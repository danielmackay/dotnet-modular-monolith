using Common.Tests.Assertions;
using Modules.Warehouse.Products.Domain;
using Modules.Warehouse.Storage.Domain;
using Modules.Warehouse.Storage.UseCases;
using Modules.Warehouse.Tests.Common;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Modules.Warehouse.Tests.Storage.UseCases;

public class AllocateStorageCommandIntegrationTests(WarehouseDatabaseFixture fixture, ITestOutputHelper output)
    : WarehouseIntegrationTestBase(fixture, output)
{
    [Fact]
    public async Task AllocateStorage_ValidRequest_ReturnsOk()
    {
        // Arrange
        var client = GetAnonymousClient();
        var product = Product.Create("Name", Sku.Create("12345678"));
        await Database.AddEntityAsync(product);
        var aisle = Aisle.Create("Name", 2, 2);
        await Database.AddEntityAsync(aisle);
        var request = new AllocateStorageCommand.Request(product.Id.Value, 5);

        // Act
        var response = await client.PostAsJsonAsync("/api/aisles/allocate-storage", request);

        // Assert
        response.ShouldHave().StatusCode(HttpStatusCode.OK);
    }
}