namespace EcommerceDDD.OrderProcessing.Domain;

public sealed class PaymentId(Guid value) : StronglyTypedId<Guid>(value)
{
    public static PaymentId Of(Guid value) => new PaymentId(value);
}
