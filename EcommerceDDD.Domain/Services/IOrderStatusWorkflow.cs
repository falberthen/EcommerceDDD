using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Services
{
    public interface IOrderStatusWorkflow
    {
        void CalculateOrderStatus(Order order, Payment payment);
    }
}
