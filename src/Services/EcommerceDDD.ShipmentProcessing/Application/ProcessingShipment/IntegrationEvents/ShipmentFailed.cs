namespace EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment.IntegrationEvents;

public class ShipmentFailed : IntegrationEvent
{
    public Guid ShipmentId { get; }
    public Guid OrderId { get; }
    public DateTime FailedAt { get; }

    public ShipmentFailed(
        Guid shipmentId,
        Guid orderId)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
        FailedAt = DateTime.UtcNow;
    }
}
