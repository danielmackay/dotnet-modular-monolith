using Ardalis.Specification;

namespace Modules.Orders.Orders.Domain.Orders;

internal class OrderSpec : Specification<Order>
{
    public OrderSpec()
    {
        Query.Include(i => i.LineItems);
    }
}
