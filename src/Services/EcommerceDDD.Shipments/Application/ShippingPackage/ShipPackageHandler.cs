using MediatR;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Shipments.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.Integration;

namespace EcommerceDDD.Shipments.Application.ShippingPackage;

public class ShipPackageHandler : ICommandHandler<ShipPackage>
{
    private readonly IEventProducer _eventProducer;
    private readonly IProductAvailabilityChecker _productAvailabilityChecker;
    private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository;

    public ShipPackageHandler(
        IEventProducer eventProducer,
        IProductAvailabilityChecker productAvailabilityChecker,
        IEventStoreRepository<Shipment> shipmentWriteRepository)
    {
        _eventProducer = eventProducer;
        _productAvailabilityChecker = productAvailabilityChecker;
        _shipmentWriteRepository = shipmentWriteRepository;
    }

    public async Task<Unit> Handle(ShipPackage command, CancellationToken cancellationToken)
    {
        var producIds = command.ProductItems
            .Select(pid => pid.ProductId.Value)
            .ToArray();

        // Checking if all items are in stock
        if(!await _productAvailabilityChecker.EnsureProductsInStock(command.ProductItems))
        {
            await _eventProducer
                   .PublishAsync(new ProductWasOutOfStock(command.OrderId.Value), cancellationToken);
            throw new ApplicationLogicException($"One of the items is out of stock");
        }

        var shipmentData = new ShipmentData(command.OrderId, command.ProductItems);
        var shipment = Shipment.Create(shipmentData);

        await _shipmentWriteRepository
            .AppendEventsAsync(shipment, cancellationToken);

        return Unit.Value;
    }
}

public record class ProductInStockViewModel(Guid ProductId, int AmountInStock);
public record class ProductStockAvailabilityRequest(Guid[] ProductIds);