namespace EcommerceDDD.Shipments.Domain.Commands;

public record class RequestShipment : ICommand
{
    public OrderId OrderId { get; private set; }
    public IReadOnlyList<ProductItem> ProductItems { get; private set; }

    public static RequestShipment Create(
        OrderId orderId,
        IReadOnlyList<ProductItem> productItems)
    {
        if (orderId is null)
            throw new ArgumentNullException(nameof(OrderId));
        if (productItems.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(productItems));

        return new RequestShipment(orderId, productItems);
    }

    private RequestShipment(
        OrderId orderId,
        IReadOnlyList<ProductItem> productItems)
    {
        OrderId = orderId;
        ProductItems = productItems;
    }
}