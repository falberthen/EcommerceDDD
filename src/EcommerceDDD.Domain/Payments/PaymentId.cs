using EcommerceDDD.Domain.SeedWork;
using System;

namespace EcommerceDDD.Domain.Payments
{
    public class PaymentId : StronglyTypedId<PaymentId>
    {
        public PaymentId(Guid value) : base(value)
        {
        }

        public static PaymentId Of(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                throw new BusinessRuleException("Quote Id must be provided.");

            return new PaymentId(paymentId);
        }
    }
}
