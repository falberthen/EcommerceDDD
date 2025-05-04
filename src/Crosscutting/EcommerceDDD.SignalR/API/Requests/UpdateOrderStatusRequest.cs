namespace EcommerceDDD.SignalR.API.Requests;

public record UpdateOrderStatusRequest(
	Guid CustomerId,
	Guid OrderId,
	string OrderStatusText,
	int OrderStatusCode
);