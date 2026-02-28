namespace EcommerceDDD.OrderProcessing.Application.Shipments.RecordingShipment;

public class RecordShipmentHandler(
	IOrderNotificationService orderNotificationService,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RecordShipment>
{
	private readonly IOrderNotificationService _orderNotificationService = orderNotificationService
		?? throw new ArgumentNullException(nameof(orderNotificationService));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(RecordShipment command, CancellationToken cancellationToken)
	{
		Activity.Current?.SetTag("order.id", command.OrderId.Value.ToString());
		await Task.Delay(TimeSpan.FromSeconds(5));

		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		if (order.Status == OrderStatus.Shipped)
			return Result.Ok();

		order.RecordShipment(command.ShipmentId);
		await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken);

		try
		{
			await _orderNotificationService.UpdateOrderStatusAsync(
				order.CustomerId.Value,
				order.Id.Value,
				order.Status.ToString(),
				(int)order.Status,
				cancellationToken);
		}
		catch (Exception)
		{
			return Result.Fail($"An error occurred when updating status for order {order.Id.Value}.");
		}

		return Result.Ok();
	}
}
