namespace EcommerceDDD.Orders.Domain;

public sealed class PaymentId : StronglyTypedId<Guid>
{
    public static PaymentId Of(Guid value)
    {
        return new PaymentId(value);
    }

    public PaymentId(Guid value) : base(value)
    {
    }
}
