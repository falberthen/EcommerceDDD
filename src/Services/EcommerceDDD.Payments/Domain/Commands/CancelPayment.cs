using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Payments.Domain.Commands;

public record class CancelPayment : ICommand
{
    public PaymentId PaymentId { get; private set; }
    public PaymentCancellationReason PaymentCancellationReason { get; private set; }

    public static CancelPayment Create(
        PaymentId paymentId,
        PaymentCancellationReason paymentCancellationReason)
    {
        if (paymentId is null)
            throw new ArgumentNullException(nameof(paymentId));

        return new CancelPayment(paymentId, paymentCancellationReason);
    }
    private CancelPayment(
        PaymentId paymentId, 
        PaymentCancellationReason paymentCancellationReason)
    {
        PaymentId = paymentId;
        PaymentCancellationReason = paymentCancellationReason;
    }
}