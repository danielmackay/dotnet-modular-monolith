namespace Modules.Customers.Customers;

/* Invariants:
 * - Must have a unique email address (handled by application)
 * - Must have an address
 */
internal class Customer : AggregateRoot<CustomerId>
{
    public string Email { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public Address? Address { get; private set; }

    private Customer() { }

    internal static Customer Create(string email, string firstName, string lastName)
    {
        var customer = new Customer { Id = new CustomerId(Uuid.Create()) };
        customer.UpdateEmail(email);
        customer.UpdateName(firstName, lastName);
        customer.AddDomainEvent(CustomerCreatedEvent.Create(customer));

        return customer;
    }

    public void UpdateName(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        FirstName = firstName;
        LastName = lastName;
    }

    public void UpdateEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        Email = email;
    }

    public void UpdateAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);
        Address = address;
    }
}