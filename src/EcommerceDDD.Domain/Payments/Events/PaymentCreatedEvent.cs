using EcommerceDDD.Domain.Core.Events;

namespace EcommerceDDD.Domain.Payments.Events;

public record class PaymentCreatedEvent : DomainEvent
{
    public PaymentId PaymentId { get; init; }

    public PaymentCreatedEvent(PaymentId paymentId)
    {
        PaymentId = paymentId;
        AggregateId = paymentId.Value;
    }
}