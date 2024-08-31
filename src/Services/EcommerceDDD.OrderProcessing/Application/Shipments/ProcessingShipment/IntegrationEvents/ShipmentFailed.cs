namespace EcommerceDDD.OrderProcessing.Application.Shipments.ProcessingShipment.IntegrationEvents;

public class ShipmentFailed(
    Guid shippingId,
    Guid orderId) : IntegrationEvent
{
    public Guid ShippingId { get; } = shippingId;
    public Guid OrderId { get; } = orderId;
    public DateTime FailedAt { get; } = DateTime.UtcNow;
}
