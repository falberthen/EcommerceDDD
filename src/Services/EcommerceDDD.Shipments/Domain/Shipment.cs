using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Shipments.Domain.Events;

namespace EcommerceDDD.Shipments.Domain;

public class Shipment : AggregateRoot<ShipmentId>
{
    public OrderId OrderId { get; private set; }
    public IReadOnlyList<ProductItem> ProductItems { get; set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public ShipmentStatus Status { get; private set; }

    public static Shipment Create(ShipmentData shipmentData)
    {
        var (OrderId, ProductItems) = shipmentData
            ?? throw new ArgumentNullException(nameof(shipmentData));

        if (OrderId is null)
            throw new BusinessRuleException("The order id is required.");

        if (ProductItems is null || shipmentData.ProductItems.Count == 0 )
            throw new BusinessRuleException("There are no products to ship.");

        return new Shipment(shipmentData);
    }

    public void RecordShipment()
    {
        var @event = PackageShipped.Create(
            Id.Value,
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(ShipmentCreated created)
    {
        Id = ShipmentId.Of(created.ShipmentId);
        OrderId = OrderId.Of(created.OrderId);
        ProductItems = created.ProductItems.Select(p =>
            new ProductItem(
                ProductId.Of(p.ProductId),
                p.Quantity)).ToList();
        CreatedAt = created.CreatedAt;
        Status = ShipmentStatus.Pending;
    }

    private void Apply(PackageShipped shipped)
    {
        ShippedAt = shipped.ShippedAt;
        Status = ShipmentStatus.Shipped;
    }

    private Shipment(ShipmentData shipmentData)
    {
        var @event = ShipmentCreated.Create(
            Guid.NewGuid(),
            shipmentData.OrderId.Value,
            shipmentData.ProductItems,
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    private Shipment() {}
}