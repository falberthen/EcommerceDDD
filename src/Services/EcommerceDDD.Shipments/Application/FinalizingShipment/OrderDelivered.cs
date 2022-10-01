using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Shipments.Application.FinalizingShipment;

public class OrderDelivered : IIntegrationEvent
{
    public Guid ShipmentId { get; private set; }
    public Guid OrderId { get; private set; }
    public DateTime DeliveredAt { get; private set; }

    public OrderDelivered(Guid shipmentId, Guid orderId)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
        DeliveredAt = DateTime.UtcNow;
    }
}