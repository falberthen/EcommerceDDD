namespace EcommerceDDD.ServiceClients.Services.Shipment;

public interface IShipmentService
{
    Task RequestShipmentAsync(Guid orderId, IList<ShipmentProductItem> items, CancellationToken cancellationToken);
}
