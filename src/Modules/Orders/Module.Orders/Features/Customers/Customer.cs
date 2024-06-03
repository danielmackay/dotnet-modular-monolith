using Ardalis.GuardClauses;
using Common.SharedKernel.Domain.Base;

namespace Module.Orders.Features.Customers;

public class Customer : AggregateRoot<CustomerId>
{
    public string Email { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public Address? Address { get; private set; }

    private Customer() { }

    public static Customer Create(string email, string firstName, string lastName)
    {
        Guard.Against.NullOrWhiteSpace(email);

        var customer = new Customer() { Id = new CustomerId(Guid.NewGuid()), Email = email, };

        customer.UpdateName(firstName, lastName);
        customer.AddDomainEvent(CustomerCreatedEvent.Create(customer));

        return customer;
    }

    public void UpdateName(string firstName, string lastName)
    {
        Guard.Against.NullOrWhiteSpace(firstName);
        Guard.Against.NullOrWhiteSpace(lastName);

        FirstName = firstName;
        LastName = lastName;
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
    }
}