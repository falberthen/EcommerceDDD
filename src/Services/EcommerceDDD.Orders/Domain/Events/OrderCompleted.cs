using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Orders.Domain.Events;

public record OrderCompleted : IDomainEvent
{
    public Guid OrderId { get; private set; }
    public Guid ShipmentId { get; private set; }
    public DateTime CompletedAt { get; private set; }

    public static OrderCompleted Create(
        Guid orderId,
        Guid shipmentId, 
        DateTime completedAt)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));
        if (completedAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(completedAt));

        return new OrderCompleted(
            orderId, 
            shipmentId,
            completedAt);
    }

    private OrderCompleted(Guid orderId, Guid shipmentId, DateTime completedAt)
    {
        OrderId = orderId;
        ShipmentId = shipmentId;
        CompletedAt = completedAt;
    }
}