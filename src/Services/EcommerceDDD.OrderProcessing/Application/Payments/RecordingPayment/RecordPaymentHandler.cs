namespace EcommerceDDD.OrderProcessing.Application.Payments.RecordingPayment;

public class RecordPaymentHandler(
	IOrderNotificationService orderNotificationService,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RecordPayment>
{
	private readonly IOrderNotificationService _orderNotificationService = orderNotificationService
		?? throw new ArgumentNullException(nameof(orderNotificationService));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(RecordPayment command, CancellationToken cancellationToken)
	{
		Activity.Current?.SetTag("order.id", command.OrderId.Value.ToString());
		await Task.Delay(TimeSpan.FromSeconds(5));

		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		if (order.Status == OrderStatus.Paid)
			return Result.Ok();

		order.RecordPayment(command.PaymentId, command.TotalPaid);

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
