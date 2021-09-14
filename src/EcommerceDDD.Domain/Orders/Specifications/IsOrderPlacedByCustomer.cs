using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.SeedWork;
using System;
using System.Linq.Expressions;

namespace EcommerceDDD.Domain.Orders.Specifications
{    
    public class IsOrderPlacedByCustomer : Specification<Order>
    {
        private CustomerId CustomerId;

        public IsOrderPlacedByCustomer(CustomerId customerId)
        {
            CustomerId = customerId;
        }

        public override Expression<Func<Order, bool>> ToExpression()
        {
            return order => order.CustomerId == CustomerId;
        }
    }
}
