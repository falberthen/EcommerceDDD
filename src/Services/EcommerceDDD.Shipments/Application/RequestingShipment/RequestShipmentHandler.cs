namespace EcommerceDDD.Shipments.Application.RequestingShipment;

public class RequestShipmentHandler : ICommandHandler<RequestShipment>
{
    private readonly ICommandBus _commandBus;
    private readonly IProductAvailabilityChecker _productAvailabilityChecker;
    private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository;
    private readonly IOutboxMessageService _outboxMessageService;

    public RequestShipmentHandler(
         ICommandBus commandBus,
        IProductAvailabilityChecker productAvailabilityChecker,
        IEventStoreRepository<Shipment> shipmentWriteRepository,
        IOutboxMessageService outboxMessageService)
    {
        _commandBus = commandBus;
        _productAvailabilityChecker = productAvailabilityChecker;
        _shipmentWriteRepository = shipmentWriteRepository;
        _outboxMessageService = outboxMessageService;
    }

    public async Task Handle(RequestShipment command, CancellationToken cancellationToken)
    {
        var producIds = command.ProductItems
            .Select(pid => pid.ProductId.Value)
            .ToArray();

        // Checking if all items are in stock
        if (!await _productAvailabilityChecker.EnsureProductsInStock(command.ProductItems))
        {
            // shipment was not created; outboxing integration event
            await _outboxMessageService.SaveAsOutboxMessageAsync(new ProductWasOutOfStock(
                command.OrderId.Value), saveChanges: true);
            throw new ApplicationLogicException($"One of the items is out of stock");
        }

        var shipmentData = new ShipmentData(command.OrderId, command.ProductItems);
        var shipment = Shipment.Create(shipmentData);
      
        await _shipmentWriteRepository
            .AppendEventsAsync(shipment, cancellationToken);

        await _commandBus.Send(ShipPackage.Create(shipment.Id));
    }
}

public record class ProductInStockViewModel(Guid ProductId, int AmountInStock);
public record class ProductStockAvailabilityRequest(Guid[] ProductIds);