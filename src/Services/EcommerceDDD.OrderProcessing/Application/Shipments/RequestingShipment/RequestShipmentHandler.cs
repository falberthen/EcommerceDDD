namespace EcommerceDDD.OrderProcessing.Application.Shipments.RequestingShipment;

public class RequestShipmentHandler : ICommandHandler<RequestShipment>
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IEventStoreRepository<Order> _orderWriteRepository;
    private readonly IConfiguration _configuration;

    public RequestShipmentHandler(
        IIntegrationHttpService integrationHttpService,
        IEventStoreRepository<Order> orderWriteRepository,
        IConfiguration configuration)
    {
        _integrationHttpService = integrationHttpService;
        _orderWriteRepository = orderWriteRepository;
        _configuration = configuration;
    }

    public async Task Handle(RequestShipment command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
            .FetchStreamAsync(command.OrderId.Value)
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        var productItemsRequest = order.OrderLines
            .Select(ol => new ProductItemRequest(
                ol.ProductItem.ProductId.Value,
                ol.ProductItem.ProductName,
                ol.ProductItem.UnitPrice.Amount,
                ol.ProductItem.Quantity))
            .ToList();

        // Requesting shippment
        var apiRoute = _configuration["ApiRoutes:ShipmentProcessing"];
        var response = await _integrationHttpService.PostAsync(apiRoute,
             new ShipOrderRequest(command.OrderId.Value, productItemsRequest));

        if (response?.Success == false)
            throw new ApplicationLogicException("An error occurred requesting shipping order.");
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