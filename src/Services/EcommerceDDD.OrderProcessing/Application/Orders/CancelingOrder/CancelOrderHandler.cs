namespace EcommerceDDD.OrderProcessing.Application.Orders.CancelingOrder;

public class CancelOrderHandler(
	IOrderNotificationService orderNotificationService,
	IEventStoreRepository<Order> orderWriteRepository,
	IMessageBus messageBus
)
{
	private readonly IOrderNotificationService _orderNotificationService = orderNotificationService
		?? throw new ArgumentNullException(nameof(orderNotificationService));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));
	private readonly IMessageBus _messageBus = messageBus
		?? throw new ArgumentNullException(nameof(messageBus));

	public async Task<Result> HandleAsync(CancelOrder command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchForWritingAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		if (order.Status == OrderStatus.Canceled)
			return Result.Ok();

		order.Cancel(command.CancellationReason);

		var orderCanceledEvent = order.GetUncommittedEvents()
			.OfType<OrderCanceled>()
			.FirstOrDefault();

		await _orderWriteRepository
			.AppendEventsAndCommitAsync(order, cancellationToken: cancellationToken);

		// Lets the saga cancel the payment when the order had already been paid
		await _messageBus.PublishAsync(orderCanceledEvent!);

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
