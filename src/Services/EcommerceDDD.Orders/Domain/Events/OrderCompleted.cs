using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Orders.Domain.Events;

public record OrderCompleted(
    OrderId OrderId, 
    DateTime CompletedAt) : IDomainEvent
{
    public static OrderCompleted Create(OrderId orderId, DateTime completedAt)
    {
        return new OrderCompleted(orderId, completedAt);
    }
}