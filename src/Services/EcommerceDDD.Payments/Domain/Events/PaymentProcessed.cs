using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Payments.Domain.Events;

public record PaymentProcessed(
    PaymentId PaymentId,
    DateTime ProcessedAt) : IDomainEvent
{
    public static PaymentProcessed Create(PaymentId paymentId, DateTime processedAt)
    {
        return new PaymentProcessed(paymentId, processedAt);
    }
}
