using ErrorOr;

namespace Modules.Orders.Orders.Domain.LineItems;

public static class LineItemErrors
{
    public static readonly Error CantRemoveAllUnits = Error.Validation(
        "LineItem.CantRemoveAllUnits",
        "Can't remove all units.  Remove the entire item instead");
}