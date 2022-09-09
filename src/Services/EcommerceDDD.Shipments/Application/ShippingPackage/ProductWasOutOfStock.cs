using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Shipments.Application.ShippingPackage;

public class ProductWasOutOfStock : IIntegrationEvent
{
    public Guid OrderId { get; private set; }
    public DateTime CheckedAt { get; private set; }

    public ProductWasOutOfStock(Guid orderId)
    {
        OrderId = orderId;
        CheckedAt = DateTime.UtcNow;
    }
}