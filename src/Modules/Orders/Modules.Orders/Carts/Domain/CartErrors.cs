using ErrorOr;

namespace Modules.Orders.Carts.Domain;

public static class CartErrors
{
    public static readonly Error NotFound = Error.Validation(
        "Cart.NotFound",
        "Cannot find the cart specified");
}
