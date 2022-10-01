using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure.Integration;

namespace EcommerceDDD.Core.Infrastructure.SignalR;

public class OrderStatusBroadcaster : IOrderStatusBroadcaster
{
    private readonly IHttpRequester _httpRequester;
    private readonly ITokenRequester _tokenRequester;
    private readonly TokenIssuerSettings _tokenIssuerSettings;
    private readonly IntegrationHttpSettings _integrationHttpSettings;

    public OrderStatusBroadcaster(
        IHttpRequester httpRequester,
        ITokenRequester tokenRequester,
        IOptions<TokenIssuerSettings> tokenIssuerSettings,
        IOptions<IntegrationHttpSettings> integrationHttpSettings)
    {
        if (tokenIssuerSettings is null)
            throw new ArgumentNullException(nameof(tokenIssuerSettings));
        if (integrationHttpSettings is null)
            throw new ArgumentNullException(nameof(integrationHttpSettings));

        _httpRequester = httpRequester;
        _tokenRequester = tokenRequester;
        _tokenIssuerSettings = tokenIssuerSettings.Value;
        _integrationHttpSettings = integrationHttpSettings.Value;
    }

    public async Task UpdateOrderStatus(UpdateOrderStatusRequest request)
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationToken(_tokenIssuerSettings);

        await _httpRequester.PostAsync<IntegrationHttpResponse>(
            $"{_integrationHttpSettings.ApiGatewayBaseUrl}/api/signalr/updateorderstatus",
            request,
            tokenResponse.AccessToken);
    }
}