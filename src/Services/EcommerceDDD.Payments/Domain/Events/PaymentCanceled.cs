namespace EcommerceDDD.Payments.Domain.Events;

public record PaymentCanceled : IDomainEvent
{
    public Guid PaymentId { get; private set; }
    public DateTime CanceledAt { get; private set; }
    public PaymentCancellationReason PaymentCancellationReason { get; private set; }

    public static PaymentCanceled Create(
        Guid paymentId,
        DateTime canceledAt,
        PaymentCancellationReason paymentCancellationReason)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (canceledAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(canceledAt));

        return new PaymentCanceled(paymentId, canceledAt, paymentCancellationReason);
    }

    private PaymentCanceled(
        Guid paymentId,
        DateTime canceledAt,
        PaymentCancellationReason paymentCancellationReason)
    {
        PaymentId = paymentId;
        CanceledAt = canceledAt;
        PaymentCancellationReason = paymentCancellationReason;
    }
}