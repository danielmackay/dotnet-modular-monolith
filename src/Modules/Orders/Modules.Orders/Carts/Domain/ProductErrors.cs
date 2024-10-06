using ErrorOr;

namespace Modules.Orders.Carts.Domain;

public static class CartErrors
{
    public static readonly Error CantRemoveMoreStockThanExists = Error.Validation(
        "Product.CantRemoveMoreStockThanExists",
        "Can't remove more stock than the warehouse has on hand");
}
