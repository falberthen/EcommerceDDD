namespace EcommerceDDD.Orders.Domain;

public sealed class OrderId : StronglyTypedId<Guid>
{
    public static OrderId Of(Guid value)
    {
        return new OrderId(value);
    }

    public OrderId(Guid value) : base(value)
    {
    }
}
