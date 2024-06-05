using Ardalis.GuardClauses;

namespace Module.Orders.Features.Customers;

internal record Address
{
    internal string Line1 { get; }
    internal string? Line2 { get; }
    internal string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    internal Address(string line1, string? line2, string city, string state, string zipCode, string country)
    {
        Guard.Against.NullOrWhiteSpace(line1);
        Guard.Against.NullOrWhiteSpace(city);
        Guard.Against.NullOrWhiteSpace(state);
        Guard.Against.NullOrWhiteSpace(zipCode);
        Guard.Against.NullOrWhiteSpace(country);

        Line1 = line1;
        Line2 = line2;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }
}
