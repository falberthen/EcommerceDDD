using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Domain.Payments.Events
{
    public class PaymentAuthorizedEvent : Event
    {
        public PaymentId PaymentId { get; private set; }

        public PaymentAuthorizedEvent(PaymentId paymentId)
        {
            PaymentId = paymentId;
            AggregateId = paymentId.Value;
        }
    }
}
