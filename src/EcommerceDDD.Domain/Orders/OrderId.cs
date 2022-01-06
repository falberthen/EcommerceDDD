using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Orders;

public sealed class OrderId : StronglyTypedId<OrderId>
{
    public OrderId(Guid value) : base(value)
    {
    }
}