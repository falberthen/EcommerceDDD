namespace EcommerceDDD.ShipmentProcessing.Application.ProcessingPayment.IntegrationEvents;

public class ProcessShipmentHandler : ICommandHandler<ProcessShipment>
{
    private readonly IProductInventoryHandler _productAvailabilityChecker;
    private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository;
    private readonly IEventPublisher _eventPublisher;

    public ProcessShipmentHandler(
        IProductInventoryHandler productAvailabilityChecker,
        IEventStoreRepository<Shipment> shipmentWriteRepository,
        IEventPublisher eventPublisher)
    {
        _productAvailabilityChecker = productAvailabilityChecker;
        _shipmentWriteRepository = shipmentWriteRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ProcessShipment command, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(3)); // 3-second delay

        var shipment = await _shipmentWriteRepository
            .FetchStreamAsync(command.ShipmentId.Value)
            ?? throw new RecordNotFoundException($"The shipment {command.ShipmentId} was not found.");

        try
        {
            // Checking if all items are in stock
            var allProductsAreAvailable = await _productAvailabilityChecker
                .CheckProductsInStockAsync(shipment.ProductItems);
            if (!allProductsAreAvailable)
                throw new BusinessRuleException($"One of the items is out of stock.");

            // Decreasing quantity in stock
            await _productAvailabilityChecker
                .DecreaseQuantityInStockAsync(shipment.ProductItems);

            // Completing shipment
            shipment.Complete();

            // Appending integration event to outbox            
            _shipmentWriteRepository.AppendToOutbox(
                new ShipmentFinalized(
                    shipment.Id.Value,
                    shipment.OrderId.Value,
                    shipment.ShippedAt!.Value));

            // Persisting aggregate
            await _shipmentWriteRepository
                .AppendEventsAsync(shipment, cancellationToken);
        }
        catch (BusinessRuleException) // Product out of stock
        {
            shipment.Cancel(ShipmentCancellationReason.ProductOutOfStock);

            // Appending integration event to outbox
            _shipmentWriteRepository.AppendToOutbox(
                new ProductWasOutOfStock(shipment.OrderId.Value));

            // Persisting domain event
            await _shipmentWriteRepository
                .AppendEventsAsync(shipment, cancellationToken);
        }
        catch (Exception) // unexpected issue
        {
            shipment.Cancel(ShipmentCancellationReason.ProcessmentError);
            
            // Appending integration event to outbox
            _shipmentWriteRepository.AppendToOutbox(
                new ShipmentFailed(shipment.Id.Value, shipment.OrderId.Value));

            // Persisting domain event
            await _shipmentWriteRepository
                .AppendEventsAsync(shipment, cancellationToken);
        }
    }
}