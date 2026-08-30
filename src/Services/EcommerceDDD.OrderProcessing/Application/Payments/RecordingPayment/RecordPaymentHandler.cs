namespace EcommerceDDD.OrderProcessing.Application.Payments.RecordingPayment;

public class RecordPaymentHandler(
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

	public async Task<Result> HandleAsync(RecordPayment command, CancellationToken cancellationToken)
	{
		await Task.Delay(TimeSpan.FromSeconds(5));

		var order = await _orderWriteRepository
			.FetchForWritingAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		// Idempotency: if already paid, re-publish OrderPaid to retry the downstream chain
		if (order.Status == OrderStatus.Paid)
		{
			await _messageBus.PublishAsync(new OrderPaid(
				order.Id.Value,
				command.PaymentId.Value,
				order.OrderLines.Select(ol => ol.ProductItem.ProductId.Value).ToList(),
				command.TotalPaid.Currency.Code,
				command.TotalPaid.Amount));

			return Result.Ok();
		}

		order.RecordPayment(command.PaymentId, command.TotalPaid);

		var orderPaidEvent = order.GetUncommittedEvents()
			.OfType<OrderPaid>()
			.FirstOrDefault();

		await _orderWriteRepository
			.AppendEventsAndCommitAsync(order, cancellationToken);

		// Tells the saga the order is paid, which is what releases the shipment request
		await _messageBus.PublishAsync(orderPaidEvent!);

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
