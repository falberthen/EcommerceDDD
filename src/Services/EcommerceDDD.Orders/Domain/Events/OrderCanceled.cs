using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Orders.Domain.Events;

public record OrderCanceled(
    OrderId OrderId,
    PaymentId PaymentId,
    OrderCancellationReason OrderCancellationReason,
    DateTime CanceledAt) : IDomainEvent
{
    public static OrderCanceled Create(
        OrderId orderId,
        PaymentId paymentId,
        OrderCancellationReason orderCancellationReason,
        DateTime canceledAt)
    {
        return new OrderCanceled(
            orderId, 
            paymentId,
            orderCancellationReason, 
            canceledAt);
    }
}
