using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Domain.Payments.Events
{
    public class PaymentCreatedEvent : Event
    {
        public PaymentId PaymentId { get; private set; }

        public PaymentCreatedEvent(PaymentId paymentId)
        {
            PaymentId = paymentId;
            AggregateId = paymentId.Value;
        }
    }
}
