namespace EcommerceDDD.OrderProcessing.Application.Orders.CancelingOrder;

public class CancelOrderHandler(
	IOrderNotificationService orderNotificationService,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<CancelOrder>
{
	private readonly IOrderNotificationService _orderNotificationService = orderNotificationService
		?? throw new ArgumentNullException(nameof(orderNotificationService));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(CancelOrder command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		order.Cancel(command.CancellationReason);
		await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken: cancellationToken);

		try
		{
			await _orderNotificationService.UpdateOrderStatusAsync(
				order.CustomerId.Value,
				command.OrderId.Value,
				order.Status.ToString(),
				(int)order.Status,
				cancellationToken);
		}
		catch (Exception)
		{
			return Result.Fail($"An error occurred when updating status for order {command.OrderId.Value}.");
		}

		return Result.Ok();
	}
}
