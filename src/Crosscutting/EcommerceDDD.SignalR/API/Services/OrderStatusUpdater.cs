namespace EcommerceDDD.SignalR.API.Services;

/// <summary>
/// Broadcasting service for Order status
/// </summary>
public class OrderStatusUpdater(
	IHubContext<OrderStatusHub, IOrderStatusHubClient> broadCastHub,
	ILogger<OrderStatusUpdater> logger) : IOrderStatusUpdater
{
	private IHubContext<OrderStatusHub, IOrderStatusHubClient> _broadCastHub { get; } = broadCastHub
		?? throw new ArgumentNullException(nameof(broadCastHub));
	private readonly ILogger<OrderStatusUpdater> _logger = logger
		?? throw new ArgumentNullException(nameof(logger));

	public async Task UpdateOrderStatusAsync(Guid customerId, Guid orderId,
		string orderStatusText, int orderStatusCode)
	{
		try
		{
			await _broadCastHub.Clients
				.Group(customerId.ToString())
				.UpdateOrderStatus(orderId.ToString(),
					orderStatusText,
					orderStatusCode
				);
		}
		catch (Exception ex)
		{
			_logger.LogError($"\n--- An error has occurred while broadcasting status for order {orderId}: {ex.Message}\n");
		}
	}
}
