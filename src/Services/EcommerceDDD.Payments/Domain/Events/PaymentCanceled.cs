using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Payments.Domain.Events;

public record PaymentCanceled(
    PaymentId PaymentId,
    PaymentCancellationReason PaymentCancellationReason) : IDomainEvent
{
    public static PaymentCanceled Create(
        PaymentId paymentId,
        PaymentCancellationReason paymentCancellationReason)
    {
        return new PaymentCanceled(paymentId, paymentCancellationReason);
    }
}