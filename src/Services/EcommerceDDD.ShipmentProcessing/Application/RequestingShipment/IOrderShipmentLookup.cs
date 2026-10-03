namespace EcommerceDDD.ShipmentProcessing.Application.RequestingShipment;

/// <summary>
/// Tells whether a shipment was already requested for an order.
/// </summary>
public interface IOrderShipmentLookup
{
	Task<bool> HasShipmentAsync(OrderId orderId, CancellationToken cancellationToken);
}
