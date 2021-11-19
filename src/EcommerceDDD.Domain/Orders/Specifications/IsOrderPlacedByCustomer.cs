using System.Linq.Expressions;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Orders.Specifications;   
public class IsOrderPlacedByCustomer : Specification<Order>
{
    private readonly CustomerId _customerId;

    public IsOrderPlacedByCustomer(CustomerId customerId)
    {
        _customerId = customerId;
    }

    public override Expression<Func<Order, bool>> ToExpression()
    {
        return order => order.CustomerId == _customerId;
    }
}