namespace EcommerceDDD.PaymentProcessing.Domain.Events;

public record PaymentCompleted : DomainEvent
{
    public Guid PaymentId { get; private set; }

    public static PaymentCompleted Create(Guid paymentId)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));

        return new PaymentCompleted(paymentId);
    }

    private PaymentCompleted(Guid paymentId)
    {
        PaymentId = paymentId;
    }
}
