using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Payments;

namespace EcommerceDDD.Domain.Services
{
    /// <summary>
    /// Domain service for calculating Order status
    /// </summary>
    public class OrderStatusWorkflow : IOrderStatusWorkflow
    {        
        public void CalculateOrderStatus(Order order, Payment payment)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (payment == null)
                throw new ArgumentNullException("payment");

            //Order Status Workflow

            if (order.Status == OrderStatus.Placed && payment.Status == PaymentStatus.ToPay)
                order.ChangeStatus(OrderStatus.WaitingForPayment);
            
            if (order.Status == OrderStatus.WaitingForPayment && payment.Status == PaymentStatus.Paid)
                order.ChangeStatus(OrderStatus.ReadyToShip);
        }        
    }
}
