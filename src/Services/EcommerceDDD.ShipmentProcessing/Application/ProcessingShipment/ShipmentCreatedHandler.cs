namespace EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment;

/// <summary>
/// Processes a shipment once it is created.
/// </summary>
public static class ShipmentCreatedHandler
{
	public static ProcessShipment Handle(ShipmentCreated @event) =>
		ProcessShipment.Create(ShipmentId.Of(@event.ShipmentId), OrderId.Of(@event.OrderId));
}
