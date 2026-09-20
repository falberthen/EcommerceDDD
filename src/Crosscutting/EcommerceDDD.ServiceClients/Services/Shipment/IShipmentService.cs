namespace EcommerceDDD.ServiceClients.Services.Shipment;

public interface IShipmentService
{
    Task RequestShipmentAsync(Guid orderId, Guid customerId, IList<ShipmentProductItem> items, CancellationToken cancellationToken);
}
