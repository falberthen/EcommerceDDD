namespace EcommerceDDD.OrderProcessing.Application.Orders.CancelingOrder;

public class CancelOrderHandler(
	SignalRClient signalrClient,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<CancelOrder>
{
	private readonly SignalRClient _signalrClient = signalrClient
		?? throw new ArgumentNullException(nameof(signalrClient));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task HandleAsync(CancelOrder command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken)
			?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

		// Canceling order
		order.Cancel(command.CancellationReason);
		await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken: cancellationToken);

		try
		{
			// Updating order status on the UI with SignalR
			var request = new UpdateOrderStatusRequest()
			{
				CustomerId = order.CustomerId.Value,
				OrderId = command.OrderId.Value,
				OrderStatusText = order.Status.ToString(),
				OrderStatusCode = (int)order.Status
			};

			var response = await _signalrClient.Api.V2.Signalr.Updateorderstatus
				.PostAsync(request, cancellationToken: cancellationToken);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred when updating status for order {command.OrderId.Value}.", ex);
		}
	}
}