using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Payments.Domain.Events;

public record PaymentRequested(
    CustomerId CustomerId,
    PaymentId PaymentId,
    OrderId OrderId,
    Money TotalAmount) : IDomainEvent
{
    public static PaymentRequested Create(
        CustomerId customerId,
        PaymentId paymentId,
        OrderId orderId,
        Money totalAmount)
    {
        return new PaymentRequested(customerId, paymentId, orderId, totalAmount);
    }
}