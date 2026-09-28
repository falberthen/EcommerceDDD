namespace EcommerceDDD.ServiceClients.Services.Notifications;

public interface IOrderNotificationService
{
	/// <summary>
	/// Pushes the order's new status to the customer's browser via SignalR
	/// </summary>
	Task UpdateOrderStatusAsync(Guid customerId, Guid orderId, string statusText, int statusCode, CancellationToken cancellationToken);
}
