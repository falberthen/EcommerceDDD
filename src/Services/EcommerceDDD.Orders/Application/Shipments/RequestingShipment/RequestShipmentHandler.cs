using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.IntegrationServices;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.IntegrationServices.Orders.Requests;
using EcommerceDDD.IntegrationServices.Shipments;
using EcommerceDDD.IntegrationServices.Shipments.Requests;
using EcommerceDDD.Orders.Domain;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.Orders.Application.Shipments.RequestingShipment;

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
        var ordersService = scopedService
           .ServiceProvider.GetRequiredService<IOrdersService>();

        var orderWriteRepository = scopedService
            .ServiceProvider.GetRequiredService<IEventStoreRepository<Order>>();

        var productItemsRequest = command.OrderLines
            .Select(ol => new ProductItemRequest(
                ol.ProductItem.ProductId.Value,
                ol.ProductItem.ProductName,
                ol.ProductItem.UnitPrice.Value,
                ol.ProductItem.Quantity))
            .ToList();

        var order = await orderWriteRepository
            .FetchStream(command.OrderId.Value);

        if (order == null)
            throw new ApplicationException($"Failed to find the order {command.OrderId}.");

        // Recording shipped event
        order.RecordShipped();

        await orderWriteRepository
            .AppendEventsAsync(order);
        
        var request = new ShipOrderRequest(command.OrderId.Value, productItemsRequest);
        await shipmentsService.RequestShipOrder(_integrationServicesSettings.ApiGatewayBaseUrl,
            request);

        // Updating order status on the UI with SignalR
        await ordersService.UpdateOrderStatus(
            _integrationServicesSettings.ApiGatewayBaseUrl,
            new UpdateOrderStatusRequest(
                order.CustomerId.Value,
                command.OrderId.Value,
                order.Status.ToString(),
                (int)order.Status));
    }
}