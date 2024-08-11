﻿namespace Modules.Orders.Orders;

internal enum OrderStatus
{
    None = 0,
    PendingPayment = 1,
    ReadyForShipping = 2,
    InTransit = 3
}
