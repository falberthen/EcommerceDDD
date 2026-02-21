namespace EcommerceDDD.OrderProcessing.Application.Orders.CompletingOrder;

public class CompleteOrderHandler(
	SignalRClient signalrClient,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<CompleteOrder>
{
	private readonly SignalRClient _signalrClient = signalrClient
		?? throw new ArgumentNullException(nameof(signalrClient));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(CompleteOrder command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		order.Complete(command.ShipmentId);

		await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken: cancellationToken);

		try
		{
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
