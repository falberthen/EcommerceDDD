namespace EcommerceDDD.Core.Infrastructure.SignalR;

public class OrderStatusBroadcaster(
	IHttpRequester httpRequester,
	ITokenRequester tokenRequester,
	IOptions<TokenIssuerSettings> tokenIssuerSettings,
	IOptions<IntegrationHttpSettings> integrationHttpSettings
) : IOrderStatusBroadcaster
{
	private readonly IHttpRequester _httpRequester = httpRequester
		?? throw new ArgumentNullException(nameof(httpRequester));
	private readonly ITokenRequester _tokenRequester = tokenRequester
		?? throw new ArgumentNullException(nameof(tokenRequester));
	private readonly TokenIssuerSettings _tokenIssuerSettings = tokenIssuerSettings?.Value
		?? throw new ArgumentNullException(nameof(tokenIssuerSettings));
	private readonly IntegrationHttpSettings _integrationHttpSettings = integrationHttpSettings?.Value
		?? throw new ArgumentNullException(nameof(integrationHttpSettings));

	public async Task UpdateOrderStatus(UpdateOrderStatusRequest request)
	{
		var tokenResponse = await _tokenRequester
			.GetApplicationTokenAsync(_tokenIssuerSettings);

		await _httpRequester.PostAsync<IntegrationHttpResponse>(
			$"{_integrationHttpSettings.ApiGatewayBaseUrl}/api/signalr/updateorderstatus",
			request,
			tokenResponse.AccessToken);
	}
}
