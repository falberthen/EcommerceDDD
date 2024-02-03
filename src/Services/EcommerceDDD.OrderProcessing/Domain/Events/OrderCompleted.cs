namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record OrderCompleted : DomainEvent
{
    public Guid OrderId { get; private set; }
    public Guid ShipmentId { get; private set; }

    public static OrderCompleted Create(
        Guid orderId,
        Guid shipmentId)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));

        return new OrderCompleted(
            orderId, 
            shipmentId);
    }

    private OrderCompleted(
        Guid orderId, 
        Guid shipmentId)
    {
        OrderId = orderId;
        ShipmentId = shipmentId;
    }
}