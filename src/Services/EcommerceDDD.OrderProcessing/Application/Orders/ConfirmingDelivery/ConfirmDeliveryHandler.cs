namespace EcommerceDDD.OrderProcessing.Application.Orders.ConfirmingDelivery;

public class ConfirmDeliveryHandler(
	IOrderNotificationService orderNotificationService,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<ConfirmDelivery>
{
	private readonly IOrderNotificationService _orderNotificationService = orderNotificationService
		?? throw new ArgumentNullException(nameof(orderNotificationService));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(ConfirmDelivery command, CancellationToken cancellationToken)
	{
		Activity.Current?.SetTag("order.id", command.OrderId.Value.ToString());
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		if (order.ShipmentId is null)
			return Result.Fail($"Order {command.OrderId} has no associated shipment.");

		order.Deliver(order.ShipmentId);

		await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken: cancellationToken);

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
