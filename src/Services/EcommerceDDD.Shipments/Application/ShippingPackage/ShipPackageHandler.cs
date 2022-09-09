using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.IntegrationServices.Products;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Shipments.Application.ShippingPackage;

public class ShipPackageHandler : CommandHandler<ShipPackage>
{
    private readonly IProductsService _productsService;
    private readonly IEventProducer _eventProducer;
    private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository;
    private readonly IntegrationServicesSettings _integrationServicesSettings;

    public ShipPackageHandler(
        IProductsService productsService,
        IEventProducer eventProducer,
        IEventStoreRepository<Shipment> shipmentWriteRepository,
        IOptions<IntegrationServicesSettings> integrationServicesSettings)
    {
        _productsService = productsService;
        _eventProducer = eventProducer;
        _shipmentWriteRepository = shipmentWriteRepository;

        if (integrationServicesSettings == null)
            throw new ArgumentNullException(nameof(integrationServicesSettings));

        _integrationServicesSettings = integrationServicesSettings.Value;
    }

    public override async Task Handle(ShipPackage command, CancellationToken cancellationToken)
    {
        // Checking if all items are in stock
        await CheckProductsAvailabilityInStock(command.OrderId, command.ProductItems, cancellationToken);

        var shipment = Shipment
            .CreateNew(command.OrderId, command.ProductItems);

        await _shipmentWriteRepository
            .AppendEventsAsync(shipment, cancellationToken);        
    }

    private async Task CheckProductsAvailabilityInStock(OrderId orderId, IReadOnlyList<ProductItem> productItems,
        CancellationToken cancellationToken)
    {
        var productIds = productItems
            .Select(i => i.ProductId.Value).ToArray();

        var productsStockAvailability = await _productsService
            .CheckProducStockAvailability(_integrationServicesSettings.ApiGatewayBaseUrl, productIds);

        if (productsStockAvailability == null)
            throw new ApplicationException("An error occurred checking products stock availability.");

        foreach (var productItem in productItems)
        {
            var productInStock = productsStockAvailability
                .Single(p => p.ProductId == productItem.ProductId.Value);

            if (productItem.Quantity > productInStock.AmountInStock)
            {
                await _eventProducer
                    .PublishAsync(new ProductWasOutOfStock(orderId.Value), cancellationToken);
                throw new ApplicationException($"Product {productItem.ProductId} is out of stock");
            }
        }
    }
}