using Microsoft.AspNetCore.SignalR;

namespace EcommerceDDD.ApiGateway.SignalR.Hubs.Order;

public interface IOrderStatusHubClient
{
    Task UpdateOrderStatus(string orderId, string orderStatusText, int orderStatusCode);
}

public class OrderStatusHub : Hub<IOrderStatusHubClient>
{
    public async Task JoinCustomerToGroup(string customerId) =>    
        await Groups.AddToGroupAsync(Context.ConnectionId, customerId);

}