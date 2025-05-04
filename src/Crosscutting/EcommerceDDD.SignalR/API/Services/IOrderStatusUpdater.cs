namespace EcommerceDDD.SignalR.API.Services;

public interface IOrderStatusUpdater
{
	Task UpdateOrderStatusAsync(Guid customerId, Guid orderId, string orderStatusText, int orderStatusCode);
}