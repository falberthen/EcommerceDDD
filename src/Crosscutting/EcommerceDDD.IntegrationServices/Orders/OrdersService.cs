using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.IntegrationServices.Base;
using EcommerceDDD.IntegrationServices.Orders.Requests;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.IntegrationServices.Orders;

public class OrdersService : IOrdersService
{
    private readonly IHttpRequester _httpRequester;
    private readonly ITokenRequester _tokenRequester;
    private readonly TokenIssuerSettings _tokenIssuerSettings;

    public OrdersService(
        IHttpRequester httpRequester,
        ITokenRequester tokenRequester,
        IOptions<TokenIssuerSettings> tokenIssuerSettings)
    {
        _httpRequester = httpRequester;
        _tokenRequester = tokenRequester;

        if (tokenIssuerSettings == null)
            throw new ArgumentNullException(nameof(tokenIssuerSettings));

        _tokenIssuerSettings = tokenIssuerSettings.Value;
    }

    public async Task<IntegrationServiceResponse> RequestPlaceOrder(string apiGatewayUrl, PlaceOrderRequest request)
    {
        // Pre-generates the order id
        var orderId = Guid.NewGuid();

        var tokenResponse = await _tokenRequester
            .GetApplicationToken(_tokenIssuerSettings);
    
        var response = await _httpRequester.PostAsync<IntegrationServiceResponse>(
            $"{apiGatewayUrl}/api/orders/{orderId}",
            request,
            tokenResponse.AccessToken);

        if (!response.Success)
            throw new Exception(response.Message);

        return response;
    }

    public async Task UpdateOrderStatus(string apiGatewayUrl, UpdateOrderStatusRequest request)
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationToken(_tokenIssuerSettings);

        await _httpRequester.PostAsync<IntegrationServiceResponse>(
            $"{apiGatewayUrl}/api/signalr/updateorderstatus",
            request,
            tokenResponse.AccessToken);
    }
}