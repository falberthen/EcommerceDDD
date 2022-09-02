using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Orders.Domain.Events;

public record OrderCanceled(
    OrderId OrderId,
    DateTime CanceledAt) : IDomainEvent
{
    public static OrderCanceled Create(
        OrderId orderId,
        DateTime canceledAt)
    {
        return new OrderCanceled(orderId, canceledAt);
    }
}
