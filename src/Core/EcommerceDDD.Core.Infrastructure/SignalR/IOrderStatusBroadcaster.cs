namespace EcommerceDDD.Core.Infrastructure.SignalR;

public interface IOrderStatusBroadcaster
{
    Task UpdateOrderStatus(UpdateOrderStatusRequest request);
}
