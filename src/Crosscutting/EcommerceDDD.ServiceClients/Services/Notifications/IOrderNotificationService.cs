namespace EcommerceDDD.ServiceClients.Services.Notifications;

public interface IOrderNotificationService
{
    Task UpdateOrderStatusAsync(Guid customerId, Guid orderId, string statusText, int statusCode, CancellationToken cancellationToken);
}
