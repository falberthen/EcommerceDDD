using EcommerceDDD.ServiceClients.SignalR;
using EcommerceDDD.ServiceClients.SignalR.Models;

namespace EcommerceDDD.ServiceClients.Services.Notifications;

public class OrderNotificationService(SignalRClient signalRClient) : IOrderNotificationService
{
    private readonly SignalRClient _signalRClient = signalRClient;

    public async Task UpdateOrderStatusAsync(Guid customerId, Guid orderId, string statusText, int statusCode, CancellationToken cancellationToken)
    {
        var request = new UpdateOrderStatusRequest()
        {
            CustomerId = customerId,
            OrderId = orderId,
            OrderStatusText = statusText,
            OrderStatusCode = statusCode
        };

        await _signalRClient.Api.V2.Signalr.Updateorderstatus
            .PostAsync(request, cancellationToken: cancellationToken);
    }
}
