using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.IntegrationServices.Base;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.IntegrationServices.Shipments.Requests;

namespace EcommerceDDD.IntegrationServices.Shipments;

public class ShipmentsService : IShipmentsService
{
    private readonly IHttpRequester _httpRequester;
    private readonly ITokenRequester _tokenRequester;
    private readonly TokenIssuerSettings _tokenIssuerSettings;

    public ShipmentsService(
        IHttpRequester httpRequester,
        ITokenRequester tokenRequester,
        IOptions<TokenIssuerSettings> tokenIssuerSettings)
    {
        _httpRequester = httpRequester;
        _tokenRequester = tokenRequester;
        _tokenIssuerSettings = tokenIssuerSettings.Value;
    }

    public async Task<IntegrationServiceResponse> RequestShipOrder(string apiGatewayUrl, ShipOrderRequest request)
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationToken(_tokenIssuerSettings);

        var response = await _httpRequester.PostAsync<IntegrationServiceResponse>(
            $"{apiGatewayUrl}/api/shipments",
            request,
            tokenResponse.AccessToken);

        if (!response.Success)
            throw new Exception(response.Message);

        return response;
    }
}