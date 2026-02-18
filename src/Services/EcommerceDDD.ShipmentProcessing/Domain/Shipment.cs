using PackageShipped = EcommerceDDD.ShipmentProcessing.Domain.Events.PackageShipped;

namespace EcommerceDDD.ShipmentProcessing.Domain;

public class Shipment : AggregateRoot<ShipmentId>
{
    public OrderId OrderId { get; private set; }
    public IReadOnlyList<ProductItem> ProductItems { get; set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public DateTime? CanceledAt { get; private set; }

    public static Shipment Create(ShipmentData shipmentData)
    {
        var (OrderId, ProductItems) = shipmentData
            ?? throw new ArgumentNullException(nameof(shipmentData));

        if (OrderId is null)
            throw new BusinessRuleException("The order id is required.");

        if (ProductItems is null || shipmentData.ProductItems.Count == 0)
            throw new BusinessRuleException("There are no products to ship.");

        return new Shipment(shipmentData);
    }

    public void Cancel(ShipmentCancellationReason shipmentCancellationReason)
    {
        if (Status == ShipmentStatus.Shipped)
            throw new BusinessRuleException($"Shipment cannot be canceled when '{Status}'");

        var @event = new ShipmentCanceled(
            Id.Value,
            shipmentCancellationReason);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Complete()
    {
        if (Status != ShipmentStatus.Pending)
            throw new BusinessRuleException($"Shipment cannot be completed when '{Status}'");

        var @event = new PackageShipped(Id.Value);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(ShipmentCreated @event)
    {
        Id = ShipmentId.Of(@event.ShipmentId);
        OrderId = OrderId.Of(@event.OrderId);
        ProductItems = @event.ProductItems.Select(p =>
            new ProductItem(
                ProductId.Of(p.ProductId),
                p.Quantity)).ToList();
        CreatedAt = @event.Timestamp;
        Status = ShipmentStatus.Pending;
    }

    private void Apply(PackageShipped @event)
    {
        Status = ShipmentStatus.Shipped;
        ShippedAt = @event.Timestamp;
    }

    private void Apply(ShipmentCanceled @event)
    {
        Status = ShipmentStatus.Canceled;
        CanceledAt = @event.Timestamp;
    }

    private Shipment(ShipmentData shipmentData)
    {
        var productItemDetails = shipmentData.ProductItems.Select(p =>
            new Events.ProductItemDetails(p.ProductId.Value, p.Quantity)).ToList();

        var @event = new ShipmentCreated(
            Guid.NewGuid(),
            shipmentData.OrderId.Value,
            productItemDetails);

        AppendEvent(@event);
        Apply(@event);
    }

    private Shipment() { }
}