using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Payments;

public sealed class PaymentId : StronglyTypedId<PaymentId>
{
    public PaymentId(Guid value) : base(value)
    {
    }
}