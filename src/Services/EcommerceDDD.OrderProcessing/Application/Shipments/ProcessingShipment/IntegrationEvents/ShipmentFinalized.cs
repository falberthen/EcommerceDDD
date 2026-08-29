namespace EcommerceDDD.OrderProcessing.Application.Shipments.ProcessingShipment.IntegrationEvents;

// Contract alias: each bounded context's copy of this event shares this wire identity.
[MessageIdentity(nameof(ShipmentFinalized))]
public class ShipmentFinalized : IntegrationEvent
{
    public Guid ShipmentId { get; set; }
    public Guid OrderId { get; set; }
    public DateTime ShippedAt { get; set; }
}