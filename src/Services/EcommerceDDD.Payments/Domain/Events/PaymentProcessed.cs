using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Payments.Domain.Events;

public record PaymentProcessed : IDomainEvent
{
    public Guid PaymentId { get; private set; }
    public DateTime ProcessedAt { get; private set; }    

    public static PaymentProcessed Create(Guid paymentId, DateTime processedAt)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (processedAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(processedAt));

        return new PaymentProcessed(paymentId, processedAt);
    }

    private PaymentProcessed(
        Guid paymentId,
        DateTime processedAt)
    {
        PaymentId = paymentId;
        ProcessedAt = processedAt;
    }
}
