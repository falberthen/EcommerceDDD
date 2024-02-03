namespace EcommerceDDD.PaymentProcessing.Domain.Events;

public record PaymentCanceled : DomainEvent
{
    public Guid PaymentId { get; private set; }
    public PaymentCancellationReason PaymentCancellationReason { get; private set; }

    public static PaymentCanceled Create(
        Guid paymentId,
        PaymentCancellationReason paymentCancellationReason)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));

        return new PaymentCanceled(paymentId, paymentCancellationReason);
    }

    private PaymentCanceled(
        Guid paymentId,
        PaymentCancellationReason paymentCancellationReason)
    {
        PaymentId = paymentId;
        PaymentCancellationReason = paymentCancellationReason;
    }
}