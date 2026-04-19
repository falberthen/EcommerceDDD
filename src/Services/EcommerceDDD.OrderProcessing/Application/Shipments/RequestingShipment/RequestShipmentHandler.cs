namespace EcommerceDDD.OrderProcessing.Application.Shipments.RequestingShipment;

public class RequestShipmentHandler(
	IShipmentService shipmentService,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RequestShipment>
{
	private readonly IShipmentService _shipmentService = shipmentService
		?? throw new ArgumentNullException(nameof(shipmentService));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(RequestShipment command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		var productItems = order.OrderLines
			.Select(ol => new ShipmentProductItem(
				ol.ProductItem.ProductId.Value,
				ol.ProductItem.ProductName,
				ol.ProductItem.Quantity,
				Convert.ToDouble(ol.ProductItem.UnitPrice.Amount)))
			.ToList();

		try
		{
			await _shipmentService.RequestShipmentAsync(order.Id.Value, productItems, cancellationToken);
			return Result.Ok();
		}
		catch (Exception)
		{
			return Result.Fail("An error occurred requesting shipping order.");
		}
	}
}
