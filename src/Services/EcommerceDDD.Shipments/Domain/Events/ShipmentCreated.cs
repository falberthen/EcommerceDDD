namespace EcommerceDDD.Shipments.Domain.Events;

public record ShipmentCreated : IDomainEvent
{
    public Guid ShipmentId { get; private set; }
    public Guid OrderId { get; private set; }
    public IReadOnlyList<ProductItemDetails> ProductItems { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static ShipmentCreated Create(
        Guid shipmentId,
        Guid orderId,
        IReadOnlyList<ProductItem> productItems,
        DateTime createdAt)
    {
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (productItems.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(productItems));
        if (createdAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(createdAt));

        var items = productItems.Select(ol => new ProductItemDetails(
            ol.ProductId.Value,
            ol.Quantity)).ToList();

        return new ShipmentCreated(
            shipmentId,
            orderId,
            items,
            createdAt);
    }

    private ShipmentCreated(
        Guid shipmentId,
        Guid orderId,
        IReadOnlyList<ProductItemDetails> productItems,
        DateTime createdAt)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
        ProductItems = productItems;
        CreatedAt = createdAt;
    }
}

public record ProductItemDetails(Guid ProductId, int Quantity);