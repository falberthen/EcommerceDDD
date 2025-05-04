using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.OrderProcessing.Application.Payments.RecordingPayment;

public class RecordPaymentHandler(
	ApiGatewayClient apiGatewayClient,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RecordPayment>
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient
		?? throw new ArgumentNullException(nameof(apiGatewayClient));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task HandleAsync(RecordPayment command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken)
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        // Recording the payment
        order.RecordPayment(command.PaymentId, command.TotalPaid);

        // Persisting aggregate
        await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken);

		try
		{
			await Task.Delay(TimeSpan.FromSeconds(5)); // 5-second delay

			// Updating order status on the UI with SignalR
			var request = new UpdateOrderStatusRequest()
			{
				CustomerId = order.CustomerId.Value,
				OrderId = order.Id.Value,
				OrderStatusText = order.Status.ToString(),
				OrderStatusCode = (int)order.Status
			};

			await _apiGatewayClient.Api.V2.Signalr.Updateorderstatus
				.PostAsync(request, cancellationToken: cancellationToken);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred when updating status for order {order.Id.Value}.", ex);
		}
	}
}