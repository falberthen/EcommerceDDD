namespace EcommerceDDD.Payments.Domain.Commands;

public record class ProcessPayment : ICommand
{
    public PaymentId PaymentId { get; private set; }

    public static ProcessPayment Create(
        PaymentId paymentId)
    {
        if (paymentId is null)
            throw new ArgumentNullException(nameof(paymentId));

        return new ProcessPayment(paymentId);
    }

    private ProcessPayment(PaymentId paymentId)
    {
        PaymentId = paymentId;
    }
}