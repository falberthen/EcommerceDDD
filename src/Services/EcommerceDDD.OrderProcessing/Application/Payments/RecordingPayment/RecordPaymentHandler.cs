namespace EcommerceDDD.OrderProcessing.Application.Payments.RecordingPayment;

public class RecordPaymentHandler(
	SignalRClient signalrClient,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RecordPayment>
{
	private readonly SignalRClient _signalrClient = signalrClient
		?? throw new ArgumentNullException(nameof(signalrClient));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(RecordPayment command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		order.RecordPayment(command.PaymentId, command.TotalPaid);

		await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken);

		try
		{
			await Task.Delay(TimeSpan.FromSeconds(5));

			var request = new UpdateOrderStatusRequest()
			{
				CustomerId = order.CustomerId.Value,
				OrderId = order.Id.Value,
				OrderStatusText = order.Status.ToString(),
				OrderStatusCode = (int)order.Status
			};

			await _signalrClient.Api.V2.Signalr.Updateorderstatus
				.PostAsync(request, cancellationToken: cancellationToken);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException)
		{
			return Result.Fail($"An error occurred when updating status for order {order.Id.Value}.");
		}

		return Result.Ok();
	}
}
