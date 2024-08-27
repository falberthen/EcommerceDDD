namespace EcommerceDDD.OrderProcessing.Domain;

public sealed class OrderId(Guid value) : StronglyTypedId<Guid>(value)
{
    public static OrderId Of(Guid value) => new OrderId(value);
}
