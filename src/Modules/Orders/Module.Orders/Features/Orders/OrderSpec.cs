using Ardalis.Specification;

namespace Module.Orders.Features.Orders;

internal class OrderSpec : Specification<Order>
{
    public OrderSpec()
    {
        Query.Include(i => i.LineItems);
    }
}
