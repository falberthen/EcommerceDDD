namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record OrderShipped : DomainEvent
{
    public Guid OrderId { get; private set; }
    public Guid ShipmentId { get; private set; }
    public IList<Guid> OrderLineProducts { get; private set; }

    public static OrderShipped Create(
        Guid orderId,
        Guid shipmentId,
        IList<Guid> orderLineProducts)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (shipmentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(shipmentId));
        if (orderLineProducts.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(orderLineProducts));

        return new OrderShipped(
            orderId,
            shipmentId,
            orderLineProducts);
    }

    private OrderShipped(
        Guid orderId,
        Guid shipmentId,
        IList<Guid> orderLineProducts)
    {
        OrderId = orderId;
        ShipmentId = shipmentId;
        OrderLineProducts = orderLineProducts;
    }
}