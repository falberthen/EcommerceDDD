using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Orders.Domain.Events;

public record class OrderShipped : IDomainEvent
{
    public Guid OrderId { get; private set; }
    public  DateTime ShippedAt { get; private set; }

    public static OrderShipped Create(
        Guid orderId, 
        DateTime shippedAt)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));        
        if (shippedAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(shippedAt));

        return new OrderShipped(
            orderId,
            shippedAt);
    }

    private OrderShipped(Guid orderId, DateTime shippedAt)
    {
        OrderId = orderId;
        ShippedAt = shippedAt;
    }
}