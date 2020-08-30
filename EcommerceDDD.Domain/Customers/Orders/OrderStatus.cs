using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Domain.Customers.Orders
{
    public enum OrderStatus
    {
        Placed = 1,
        WaitingForPayment = 2,
        Paid = 3,
        Canceled = 0,
    }
}
