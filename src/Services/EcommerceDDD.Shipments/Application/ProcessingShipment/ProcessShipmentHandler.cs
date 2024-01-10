using EcommerceDDD.Payments.Application.ProcessingPayment;

namespace EcommerceDDD.Shipments.Application.ProcessingShipment;

public class ProcessShipmentHandler : ICommandHandler<ProcessShipment>
{
    private readonly IProductAvailabilityChecker _productAvailabilityChecker;
    private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository;

    public ProcessShipmentHandler(         
        IProductAvailabilityChecker productAvailabilityChecker,
        IEventStoreRepository<Shipment> shipmentWriteRepository)
    {        
        _productAvailabilityChecker = productAvailabilityChecker;
        _shipmentWriteRepository = shipmentWriteRepository;        
    }

    public async Task Handle(ProcessShipment command, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentWriteRepository
            .FetchStreamAsync(command.ShipmentId.Value)
            ?? throw new RecordNotFoundException($"The shipment {command.ShipmentId} was not found.");

        try
        {
            // Checking if all items are in stock
            if (!await _productAvailabilityChecker.EnsureProductsInStock(shipment.ProductItems))
                throw new ApplicationLogicException($"One of the items is out of stock");

            // Recording shipment
            shipment.RecordShipment();

            // Persisting events
            _shipmentWriteRepository.AppendIntegrationEvent(
                new PackageShipped(
                    shipment.Id.Value,
                    shipment.OrderId.Value,
                    shipment.ShippedAt!.Value)
            );
            await _shipmentWriteRepository
                .AppendEventsAsync(shipment, cancellationToken);            
        }
        catch (BusinessRuleException) // Product out of stock
        {
            shipment.Cancel(ShipmentCancellationReason.ProductOutOfStock);

            // Persisting events
            _shipmentWriteRepository.AppendIntegrationEvent(
                new ProductWasOutOfStock(shipment.OrderId.Value));
            await _shipmentWriteRepository.AppendEventsAsync(shipment, cancellationToken);            
        }
        catch (Exception) // unexpected issue
        {            
            shipment.Cancel(ShipmentCancellationReason.ProcessmentError);

            // Persisting events
            await _shipmentWriteRepository.AppendEventsAsync(shipment, cancellationToken);
            _shipmentWriteRepository.AppendIntegrationEvent(
                new ShipmentFailed(shipment.Id.Value,shipment.OrderId.Value));
        }
    }
}