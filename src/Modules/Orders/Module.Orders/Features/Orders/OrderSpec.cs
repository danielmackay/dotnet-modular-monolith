using Ardalis.Specification;

namespace Module.Orders.Features.Orders;

public class OrderSpec : Specification<Order>
{
    public OrderSpec()
    {
        Query.Include(i => i.LineItems);
    }
}