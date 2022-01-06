using System.Threading.Tasks;
using EcommerceDDD.Domain.Orders;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Application.Core.SignalR;

namespace EcommerceDDD.Application.Orders;

public interface IOrderStatusBroadcaster
{
    Task BroadcastOrderStatus(CustomerId customerId, OrderId orderId, OrderStatus orderStatus);
}

/// <summary>
/// Broadcasting service for Order status
/// </summary>
public class OrderStatusBroadcaster : IOrderStatusBroadcaster
{
    private readonly IHubContext<OrderStatusHub, IOrderStatusHubClient> _broadCastHub;
    private readonly ILogger<OrderStatusBroadcaster> _logger;

    public OrderStatusBroadcaster(
        IHubContext<OrderStatusHub, IOrderStatusHubClient> broadCastHub,
        ILogger<OrderStatusBroadcaster> logger)
    {
        _broadCastHub = broadCastHub;
        _logger = logger;
    }

    public async Task BroadcastOrderStatus(
        CustomerId customerId, 
        OrderId orderId, 
        OrderStatus orderStatus)
    {
        try
        {
            var prettyStatus = OrderStatusPrettier
                .Prettify(orderStatus);

            await _broadCastHub.Clients
                .Groups(customerId.Value.ToString())
                .UpdateOrderStatus(
                    orderId.Value.ToString(),
                    prettyStatus
                );
        }
        catch (Exception ex)
        {
            _logger.LogError($"\n--- An error has occurred while broadcasting status for order {orderId.Value}: {ex.Message}\n");
        }
    }
}