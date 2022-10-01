using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Application.Payments.CancelingPayment;

public record class RequestCancelPayment : ICommand
{
    public PaymentId PaymentId { get; private set; }
    public PaymentCancellationReason PaymentCancellationReason { get; private set; }

    public static RequestCancelPayment Create(
        PaymentId paymentId,
        PaymentCancellationReason paymentCancellationReason)
    {
        if (paymentId is null)
            throw new ArgumentNullException(nameof(paymentId));

        return new RequestCancelPayment(paymentId, paymentCancellationReason);
    }
    
    private RequestCancelPayment(
        PaymentId paymentId, 
        PaymentCancellationReason paymentCancellationReason)
    {
        PaymentId = paymentId;
        PaymentCancellationReason = paymentCancellationReason;
    }
}