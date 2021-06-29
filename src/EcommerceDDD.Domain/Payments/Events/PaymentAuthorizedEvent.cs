using EcommerceDDD.Domain.Core.Events;

namespace EcommerceDDD.Domain.Payments.Events
{
    public class PaymentAuthorizedEvent : DomainEvent
    {
        public PaymentId PaymentId { get; private set; }

        public PaymentAuthorizedEvent(PaymentId paymentId)
        {
            PaymentId = paymentId;
            AggregateId = paymentId.Value;
        }
    }
}
