using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Shipments.Domain.Events;

public record PackageShipped : IDomainEvent
{
    public Guid ShipmentId { get; private set; }
    public Guid OrderId { get; private set; }
    public IReadOnlyList<ProductItemDetails> ProductItems { get; private set; }
    public DateTime ShippedAt { get; private set; }

    public static PackageShipped Create(
        Guid shipmentId,
        Guid orderId,
        IReadOnlyList<ProductItem> productItems,
        DateTime shippedAt)
    {
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (productItems.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(productItems));
        if (shippedAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(shippedAt));

        var items = productItems.Select(ol => new ProductItemDetails(
            ol.ProductId.Value,
            ol.Quantity)).ToList();

        return new PackageShipped(
            shipmentId,
            orderId,
            items,
            shippedAt);
    }

    private PackageShipped(
        Guid shipmentId,
        Guid orderId,
        IReadOnlyList<ProductItemDetails> productItems,
        DateTime shippedAt)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
        ProductItems = productItems;
        ShippedAt = shippedAt;
    }
}

public record ProductItemDetails(Guid ProductId, int Quantity);