namespace EcommerceDDD.ApiGateway.SignalR.Hubs.Order;

public interface IOrderStatusHubClient
{
    Task UpdateOrderStatus(string orderId, string orderStatusText, int orderStatusCode);
}

[Authorize(Roles = Roles.Customer)]
public class OrderStatusHub : Hub<IOrderStatusHubClient>
{
    /// <summary>
    /// Subscribes the caller to their own order notifications.
    /// </summary>
    public async Task JoinCustomerToGroup()
    {
        var customerId = Context.User?.FindFirstValue(CustomClaimTypes.CustomerId);

        if (string.IsNullOrWhiteSpace(customerId))
            throw new HubException("The authenticated user is not linked to a customer.");

        await Groups.AddToGroupAsync(Context.ConnectionId, customerId);
    }
}
