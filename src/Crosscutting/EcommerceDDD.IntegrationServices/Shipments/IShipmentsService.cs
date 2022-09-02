using EcommerceDDD.IntegrationServices.Base;
using EcommerceDDD.IntegrationServices.Shipments.Requests;

namespace EcommerceDDD.IntegrationServices.Shipments;

public interface IShipmentsService
{
    Task<IntegrationServiceResponse> RequestShipOrder(string apiGatewayUrl, ShipOrderRequest request);
}
