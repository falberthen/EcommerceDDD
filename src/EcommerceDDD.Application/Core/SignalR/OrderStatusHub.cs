using EcommerceDDD.Application.Orders.GetOrderDetails;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EcommerceDDD.Application.Core.SignalR
{
    public interface IOrderStatusHubClient
    {
        Task UpdateOrderStatus(string orderId, OrderStatusViewModel orderStatus);
    }

    public class OrderStatusHub : Hub<IOrderStatusHubClient>
    {
        public Task JoinCustomerToGroup(string customerId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, customerId);
        }
    }    
}
