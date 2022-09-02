using EcommerceDDD.IntegrationServices.Base;
using EcommerceDDD.IntegrationServices.Orders.Requests;

namespace EcommerceDDD.IntegrationServices.Orders;

public interface IOrdersService
{
    Task<IntegrationServiceResponse> RequestPlaceOrder(string apiGatewayUrl, PlaceOrderRequest request);
    Task UpdateOrderStatus(string apiGatewayUrl, UpdateOrderStatusRequest request);
}
