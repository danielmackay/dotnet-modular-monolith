using Ardalis.Specification;

namespace Modules.Orders.Carts.Domain;

internal class CartByIdSpec : Specification<Cart>
{
    public CartByIdSpec(CartId id) : base()
    {
        Query
            .Where(i => i.Id == id)
            .Include(i => i.Items);
    }
}
