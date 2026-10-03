namespace EcommerceDDD.ShipmentProcessing.Application.RequestingShipment;

public class OrderShipmentLookup(IQuerySession querySession) : IOrderShipmentLookup
{
	private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

	public Task<bool> HasShipmentAsync(OrderId orderId, CancellationToken cancellationToken) =>
		_querySession.Query<ShipmentDetails>()
			.AnyAsync(s => s.OrderId == orderId.Value, cancellationToken);
}
