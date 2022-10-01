using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Orders.Domain.Events;

public record class OrderCanceled : IDomainEvent
{
    public Guid OrderId { get; private set; }
    public Guid? PaymentId { get; private set; }
    public OrderCancellationReason OrderCancellationReason { get; private set; }
    public DateTime CanceledAt { get; private set; }

    public static OrderCanceled Create(
        Guid orderId,
        Guid? paymentId,
        OrderCancellationReason orderCancellationReason,
        DateTime canceledAt)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));        
        if (canceledAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(canceledAt));

        return new OrderCanceled(
            orderId, 
            paymentId,
            orderCancellationReason, 
            canceledAt);
    }

    private OrderCanceled(
        Guid orderId,
        Guid? paymentId,
        OrderCancellationReason orderCancellationReason,
        DateTime canceledAt)
    {
        OrderId = orderId;
        PaymentId = paymentId;
        OrderCancellationReason = orderCancellationReason;
        CanceledAt = canceledAt;
    }
}
