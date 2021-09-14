using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.SeedWork;
using System;
using System.Linq.Expressions;

namespace EcommerceDDD.Domain.Payments.Specifications
{    
    public class IsPaymentOfOrder : Specification<Payment>
    {
        private OrderId OrderId;

        public IsPaymentOfOrder(OrderId orderId)
        {
            OrderId = orderId;
        }

        public override Expression<Func<Payment, bool>> ToExpression()
        {
            return payment => payment.OrderId == OrderId;
        }
    }
}
