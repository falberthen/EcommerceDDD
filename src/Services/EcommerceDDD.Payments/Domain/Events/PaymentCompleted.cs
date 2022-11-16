using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Payments.Domain.Events;

public record PaymentCompleted : IDomainEvent
{
    public Guid PaymentId { get; private set; }
    public DateTime CompletedAt { get; private set; }    

    public static PaymentCompleted Create(Guid paymentId, DateTime processedAt)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (processedAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(processedAt));

        return new PaymentCompleted(paymentId, processedAt);
    }

    private PaymentCompleted(
        Guid paymentId,
        DateTime processedAt)
    {
        PaymentId = paymentId;
        CompletedAt = processedAt;
    }
}
