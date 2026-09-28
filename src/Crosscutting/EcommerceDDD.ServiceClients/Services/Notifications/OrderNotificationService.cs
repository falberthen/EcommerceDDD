namespace EcommerceDDD.ServiceClients.Services.Notifications;

public class OrderNotificationService(
	SignalRClient signalRClient,
	ILogger<OrderNotificationService> logger) : IOrderNotificationService
{
	private readonly SignalRClient _signalRClient = signalRClient;
	private readonly ILogger<OrderNotificationService> _logger = logger;

	public async Task UpdateOrderStatusAsync(Guid customerId, Guid orderId, 
		string statusText, int statusCode, CancellationToken cancellationToken)
	{
		var request = new UpdateOrderStatusRequest()
		{
			CustomerId = customerId,
			OrderId = orderId,
			OrderStatusText = statusText,
			OrderStatusCode = statusCode
		};

		try
		{
			await _signalRClient.Api.V2.Signalr.Updateorderstatus
				.PostAsync(request, cancellationToken: cancellationToken);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Could not push status {Status} of order {OrderId}.", statusText, orderId);
		}
	}
}
