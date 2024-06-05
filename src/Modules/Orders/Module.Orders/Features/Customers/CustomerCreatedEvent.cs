using Common.SharedKernel.Domain.Base;

namespace Module.Orders.Features.Customers;

internal record CustomerCreatedEvent(CustomerId Id, string FirstName, string LastName) : DomainEvent
{
    public static CustomerCreatedEvent Create(Customer customer) =>
        new CustomerCreatedEvent(customer.Id, customer.FirstName, customer.LastName);
}
