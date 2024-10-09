using Modules.Customers.Customers.Domain;

namespace Modules.Customers.Tests;

public class CustomerTests
{
    [Fact]
    public void Create_ShouldInitializeCustomerWithGivenValues()
    {
        // Arrange
        var email = "test@example.com";
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var customer = Customer.Create(email, firstName, lastName);

        // Assert
        customer.Email.Should().Be(email);
        customer.FirstName.Should().Be(firstName);
        customer.LastName.Should().Be(lastName);
        customer.Address.Should().BeNull();
    }

    [Fact]
    public void UpdateName_ShouldUpdateFirstNameAndLastName()
    {
        // Arrange
        var customer = Customer.Create("test@example.com", "John", "Doe");
        var newFirstName = "Jane";
        var newLastName = "Smith";

        // Act
        customer.UpdateName(newFirstName, newLastName);

        // Assert
        customer.FirstName.Should().Be(newFirstName);
        customer.LastName.Should().Be(newLastName);
    }

    [Fact]
    public void UpdateEmail_ShouldUpdateEmail()
    {
        // Arrange
        var customer = Customer.Create("test@example.com", "John", "Doe");
        var newEmail = "new@example.com";

        // Act
        customer.UpdateEmail(newEmail);

        // Assert
        customer.Email.Should().Be(newEmail);
    }

    [Fact]
    public void UpdateAddress_ShouldUpdateAddress()
    {
        // Arrange
        var customer = Customer.Create("test@example.com", "John", "Doe");
        var address = new Address("123 Main St", null, "City", "State", "12345", "US");

        // Act
        customer.UpdateAddress(address);

        // Assert
        customer.Address.Should().Be(address);
    }
}