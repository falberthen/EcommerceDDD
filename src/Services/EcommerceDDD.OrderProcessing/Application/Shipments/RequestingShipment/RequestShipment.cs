namespace EcommerceDDD.OrderProcessing.Application.Shipments.RequestingShipment;

public record class RequestShipment : ICommand, ITraceable
{
    public OrderId OrderId { get; private set; }

    public static RequestShipment Create(OrderId orderId)
    {
        if (orderId is null)
            throw new ArgumentNullException(nameof(orderId));

        return new RequestShipment(orderId);
    }

    public IEnumerable<KeyValuePair<string, string>> GetSpanTags() =>
        [new("order.id", OrderId.Value.ToString())];

    private RequestShipment(OrderId orderId) => OrderId = orderId;
}