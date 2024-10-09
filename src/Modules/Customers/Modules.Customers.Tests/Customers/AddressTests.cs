using Modules.Customers.Customers.Domain;

namespace Modules.Customers.Tests;

public class AddressTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties_WhenValidArguments()
    {
        // Arrange
        var line1 = "123 Main St";
        var line2 = "Apt 4B";
        var city = "Anytown";
        var state = "CA";
        var zipCode = "12345";
        var country = "USA";

        // Act
        var address = new Address(line1, line2, city, state, zipCode, country);

        // Assert
        address.Line1.Should().Be(line1);
        address.Line2.Should().Be(line2);
        address.City.Should().Be(city);
        address.State.Should().Be(state);
        address.ZipCode.Should().Be(zipCode);
        address.Country.Should().Be(country);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenLine1IsNullOrWhiteSpace()
    {
        // Arrange
        var line1 = "";
        var city = "Anytown";
        var state = "CA";
        var zipCode = "12345";
        var country = "USA";

        // Act
        Action act = () => _ = new Address(line1, null, city, state, zipCode, country);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenCityIsNullOrWhiteSpace()
    {
        // Arrange
        var line1 = "123 Main St";
        var city = "";
        var state = "CA";
        var zipCode = "12345";
        var country = "USA";

        // Act
        Action act = () => _ = new Address(line1, null, city, state, zipCode, country);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenStateIsNullOrWhiteSpace()
    {
        // Arrange
        var line1 = "123 Main St";
        var city = "Anytown";
        var state = "";
        var zipCode = "12345";
        var country = "USA";

        // Act
        Action act = () => _ = new Address(line1, null, city, state, zipCode, country);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenZipCodeIsNullOrWhiteSpace()
    {
        // Arrange
        var line1 = "123 Main St";
        var city = "Anytown";
        var state = "CA";
        var zipCode = "";
        var country = "USA";

        // Act
        Action act = () => _ = new Address(line1, null, city, state, zipCode, country);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenCountryIsNullOrWhiteSpace()
    {
        // Arrange
        var line1 = "123 Main St";
        var city = "Anytown";
        var state = "CA";
        var zipCode = "12345";
        var country = "";

        // Act
        Action act = () => _ = new Address(line1, null, city, state, zipCode, country);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
