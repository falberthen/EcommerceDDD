namespace EcommerceDDD.IntegrationServices.Orders.Requests;

public record UpdateOrderStatusRequest(Guid CustomerId, Guid OrderId, string OrderStatusText, int OrderStatusCode);