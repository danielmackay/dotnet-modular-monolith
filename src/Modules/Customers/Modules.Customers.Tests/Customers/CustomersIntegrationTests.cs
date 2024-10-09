using Common.Tests.Assertions;
using Microsoft.EntityFrameworkCore;
using Modules.Customers.Customers.Domain;
using Modules.Customers.Customers.UseCases;
using Modules.Customers.Tests.Common;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Modules.Customers.Tests.Customers;

public class CustomersIntegrationTests(CustomersDatabaseFixture fixture, ITestOutputHelper output)
    : CustomersIntegrationTestBase(fixture, output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task RegisterCustomer_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = GetAnonymousClient();
        var request = new RegisterCustomerCommand.Request("first", "last", "email@foo.com");

        // Act
        var response = await client.PostAsJsonAsync("/api/customers", request);

        // Assert
        HttpContentExtensions.Should(response).BeStatusCode(HttpStatusCode.Created);
        var customers = await GetQueryable<Customer>().ToListAsync();
        customers.Should().HaveCount(1);

        var customer = customers.First();
        customer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        customer.CreatedBy.Should().NotBeNullOrWhiteSpace();
        customer.FirstName.Should().Be(request.FirstName);
        customer.LastName.Should().Be(request.LastName);
        customer.Email.Should().Be(request.Email);
    }

    [Theory]
    [InlineData(null, "last", "email@foo.com")]
    [InlineData("", "last", "email@foo.com")]
    [InlineData(" ", "last", "email@foo.com")]
    [InlineData("first", null, "email@foo.com")]
    [InlineData("first", "", "email@foo.com")]
    [InlineData("first", " ", "email@foo.com")]
    [InlineData("first", "last", null)]
    [InlineData("first", "last", "")]
    [InlineData("first", "last", " ")]
    [InlineData("first", "last", "email")]
    public async Task RegisterCustomer_InvalidRequest_ReturnsBadRequest(string? firstName, string? lastName, string? email)
    {
        // Arrange
        var client = GetAnonymousClient();
        var request = new RegisterCustomerCommand.Request(firstName!, lastName!, email!);

        // Act
        var response = await client.PostAsJsonAsync("/api/customers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine(content);
    }
}
