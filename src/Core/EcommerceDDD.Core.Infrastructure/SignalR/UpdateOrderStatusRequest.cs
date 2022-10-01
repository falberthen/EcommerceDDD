namespace EcommerceDDD.Core.Infrastructure.SignalR;

public record UpdateOrderStatusRequest(Guid CustomerId, Guid OrderId, string OrderStatusText, int OrderStatusCode);