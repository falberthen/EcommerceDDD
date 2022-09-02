using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Shipments.Domain.Events;

namespace EcommerceDDD.Shipments.Domain;

public class Shipment : AggregateRoot<ShipmentId>
{
    public OrderId OrderId { get; private set; }
    public IReadOnlyList<ProductItem> ProductItems { get; set; } = default!;
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public ShipmentStatus Status { get; private set; }

    public static Shipment CreateNew(OrderId orderId, IReadOnlyList<ProductItem> productItems)
    {
        if (orderId == null)
            throw new DomainException("The order id is required.");

        if (productItems == null || productItems.Count == 0 )
            throw new DomainException("There are no products to ship.");

        return new Shipment(orderId, productItems);
    }

    public PackageDelivered RecordDelivery()
    {        
        var @event = PackageDelivered
            .Create(Id, OrderId, DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
        return @event;
    }

    private void Apply(PackageShipped shipped)
    {
        Id = shipped.ShipmentId;
        OrderId = shipped.OrderId;
        ProductItems = shipped.ProductItems;
        ShippedAt = shipped.ShippedAt;
        Status = ShipmentStatus.Shipped;
    }

    private void Apply(PackageDelivered delivered)
    {
        DeliveredAt = delivered.DeliveredAt;
        Status = ShipmentStatus.Delivered;
    }

    private Shipment(OrderId orderId, IReadOnlyList<ProductItem> products)
    {
        var @event = PackageShipped.Create(
            ShipmentId.Of(Guid.NewGuid()),
            orderId,
            products,
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    private Shipment() { }
}