using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.IntegrationServices;
using EcommerceDDD.IntegrationServices.Shipments;
using EcommerceDDD.IntegrationServices.Shipments.Requests;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.Orders.Application.Orders.RequestingShipment;

public class RequestShipmentHandler : CommandHandler<RequestShipment>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IntegrationServicesSettings _integrationServicesSettings;

    public RequestShipmentHandler(
        IServiceProvider serviceProvider,
        IOptions<IntegrationServicesSettings> integrationServicesSettings)
    {
        if (integrationServicesSettings == null)
            throw new ArgumentNullException(nameof(integrationServicesSettings));

        _serviceProvider = serviceProvider;
        _integrationServicesSettings = integrationServicesSettings.Value;
    }

    public override async Task Handle(RequestShipment command, CancellationToken cancellationToken)
    {
        if (_serviceProvider == null)
            throw new ArgumentNullException(nameof(_serviceProvider));

        using var scopedService = _serviceProvider.CreateScope();
        var shipmentsService = scopedService
           .ServiceProvider.GetRequiredService<IShipmentsService>();

        var productItemsRequest = command.OrderLines
            .Select(ol => new ProductItemRequest(
                ol.ProductItem.ProductId.Value,
                ol.ProductItem.ProductName,
                ol.ProductItem.UnitPrice.Value,
                ol.ProductItem.Quantity))
            .ToList();

        var request = new ShipOrderRequest(command.OrderId.Value, productItemsRequest);
        await shipmentsService.RequestShipOrder(_integrationServicesSettings.ApiGatewayBaseUrl,
            request);
    }
}