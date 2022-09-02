using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Payments.Domain.Events;

public record PaymentRequested(
    PaymentId PaymentId,
    OrderId OrderId,
    Money TotalAmount) : IDomainEvent
{
    public static PaymentRequested Create(
        PaymentId paymentId,
        OrderId orderId,
        Money totalAmount)
    {
        return new PaymentRequested(paymentId, orderId, totalAmount);
    }
}