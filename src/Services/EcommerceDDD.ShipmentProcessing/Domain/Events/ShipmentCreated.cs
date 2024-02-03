namespace EcommerceDDD.ShipmentProcessing.Domain.Events;

public record ShipmentCreated : DomainEvent
{
    public Guid ShipmentId { get; private set; }
    public Guid OrderId { get; private set; }
    public IReadOnlyList<ProductItemDetails> ProductItems { get; private set; }

    public static ShipmentCreated Create(
        Guid shipmentId,
        Guid orderId,
        IReadOnlyList<ProductItem> productItems)
    {
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (productItems.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(productItems));

        var items = productItems.Select(ol => new ProductItemDetails(
            ol.ProductId.Value,
            ol.Quantity)).ToList();

        return new ShipmentCreated(
            shipmentId,
            orderId,
            items);
    }

    private ShipmentCreated(
        Guid shipmentId,
        Guid orderId,
        IReadOnlyList<ProductItemDetails> productItems)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
        ProductItems = productItems;
    }
}

public record ProductItemDetails(Guid ProductId, int Quantity);