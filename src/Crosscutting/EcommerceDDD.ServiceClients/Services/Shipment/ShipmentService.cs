using EcommerceDDD.ServiceClients.ShipmentProcessing;
using EcommerceDDD.ServiceClients.ShipmentProcessing.Models;

namespace EcommerceDDD.ServiceClients.Services.Shipment;

public class ShipmentService(ShipmentProcessingClient shipmentProcessingClient) : IShipmentService
{
    private readonly ShipmentProcessingClient _shipmentProcessingClient = shipmentProcessingClient;

    public async Task RequestShipmentAsync(Guid orderId, IList<ShipmentProductItem> items, CancellationToken cancellationToken)
    {
        var productItems = items.Select(i => new ProductItemRequest()
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList();

        var request = new ShipOrderRequest()
        {
            OrderId = orderId,
            ProductItems = productItems
        };

        await _shipmentProcessingClient.Api.V2.Internal.Shipments
            .PostAsync(request, cancellationToken: cancellationToken);
    }
}
