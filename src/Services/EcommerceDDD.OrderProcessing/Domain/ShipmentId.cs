namespace EcommerceDDD.OrderProcessing.Domain;

public sealed class ShipmentId(Guid value) : StronglyTypedId<Guid>(value)
{
    public static ShipmentId Of(Guid value) => new ShipmentId(value);
}
