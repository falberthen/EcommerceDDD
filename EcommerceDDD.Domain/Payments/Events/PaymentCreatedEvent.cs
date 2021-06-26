using EcommerceDDD.Domain.Core.Events;

namespace EcommerceDDD.Domain.Payments.Events
{
    public class PaymentCreatedEvent : DomainEvent
    {
        public PaymentId PaymentId { get; private set; }

        public PaymentCreatedEvent(PaymentId paymentId)
        {
            PaymentId = paymentId;
            AggregateId = paymentId.Value;
        }
    }
}
