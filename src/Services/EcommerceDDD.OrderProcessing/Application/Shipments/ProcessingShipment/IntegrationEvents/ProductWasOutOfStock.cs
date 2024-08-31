namespace EcommerceDDD.OrderProcessing.Application.Shipments.ProcessingShipment.IntegrationEvents;

public class ProductWasOutOfStock(Guid orderId) : IntegrationEvent
{
    public Guid OrderId { get; private set; } = orderId;
    public DateTime CheckedAt { get; private set; } = DateTime.UtcNow;
}