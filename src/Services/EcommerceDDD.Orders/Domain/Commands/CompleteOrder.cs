namespace EcommerceDDD.Orders.Domain.Commands;

public record class CompleteOrder : ICommand
{
    public OrderId OrderId { get; private set; }
    public ShipmentId ShipmentId { get; private set; }

    public static CompleteOrder Create(
       OrderId orderId,
       ShipmentId shipmentId)
    {
        if (orderId is null)
            throw new ArgumentNullException(nameof(orderId));
        if (shipmentId is null)
            throw new ArgumentNullException(nameof(shipmentId));

        return new CompleteOrder(orderId, shipmentId);
    }

    private CompleteOrder(
        OrderId orderId,
        ShipmentId shipmentId)
    {
        OrderId = orderId;
        ShipmentId = shipmentId;
    }
}