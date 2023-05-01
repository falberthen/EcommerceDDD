namespace EcommerceDDD.Orders.Application.Shipments.RequestingShipment;

public class RequestShipmentHandler : ICommandHandler<RequestShipment>
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IOrderStatusBroadcaster _orderStatusBroadcaster;
    private readonly IEventStoreRepository<Order> _orderWriteRepository;

    public RequestShipmentHandler(
        IIntegrationHttpService integrationHttpService,
        IOrderStatusBroadcaster orderStatusBroadcaster,        
        IEventStoreRepository<Order> orderWriteRepository)
    {
        _integrationHttpService = integrationHttpService;
        _orderStatusBroadcaster = orderStatusBroadcaster;        
        _orderWriteRepository = orderWriteRepository;
    }

    public async Task Handle(RequestShipment command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
            .FetchStreamAsync(command.OrderId.Value);

        if (order is null)
            throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        var productItemsRequest = order.OrderLines
            .Select(ol => new ProductItemRequest(
                ol.ProductItem.ProductId.Value,
                ol.ProductItem.ProductName,
                ol.ProductItem.UnitPrice.Amount,
                ol.ProductItem.Quantity))
            .ToList();

        // Requesting shippment
        var request = new ShipOrderRequest(command.OrderId.Value, productItemsRequest);
        var response = await _integrationHttpService.PostAsync("api/shipments",
             new ShipOrderRequest(command.OrderId.Value, productItemsRequest));

        if (response is null || !response!.Success)
            throw new ApplicationLogicException("An error occurred requesting shipping order.");

        // Recording shipped event
        order.RecordShippedEvent();
        await _orderWriteRepository
            .AppendEventsAsync(order);

        // Updating order status on the UI with SignalR
        await _orderStatusBroadcaster.UpdateOrderStatus(
            new UpdateOrderStatusRequest(
                order.CustomerId.Value,
                command.OrderId.Value,
                order.Status.ToString(),
                (int)order.Status));
    }
}

public record class ShipOrderRequest(
    Guid OrderId,
    IReadOnlyList<ProductItemRequest> ProductItems);

public record ProductItemRequest(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity);