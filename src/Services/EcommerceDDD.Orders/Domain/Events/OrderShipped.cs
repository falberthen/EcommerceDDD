using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Orders.Domain.Events;

public record class OrderShipped(
    OrderId OrderId,
    DateTime ShippedAt) : IDomainEvent
{
    public static OrderShipped Create(
        OrderId orderId, 
        DateTime shippedAt)
    {
        return new OrderShipped(
            orderId,
            shippedAt);
    }
}