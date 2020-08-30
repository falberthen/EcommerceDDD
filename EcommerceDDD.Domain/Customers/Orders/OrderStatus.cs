using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Domain.Customers.Orders
{
    public enum OrderStatus
    {
        Placed = 1,
        WaitingForPayment = 2,
        Sent = 3,
        Delivered = 4,
        Canceled = 0,
    }
}
