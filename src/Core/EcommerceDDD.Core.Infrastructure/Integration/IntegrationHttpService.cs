namespace EcommerceDDD.Core.Infrastructure.Integration;

public class IntegrationHttpService(
	IHttpRequester httpRequester,
	ITokenRequester tokenRequester,
	IOptions<TokenIssuerSettings> tokenIssuerSettings,
	IOptions<IntegrationHttpSettings> integrationSettings) : IIntegrationHttpService
{
    private readonly IHttpRequester _httpRequester = httpRequester 
		?? throw new ArgumentNullException(nameof(httpRequester));
    private readonly ITokenRequester _tokenRequester = tokenRequester
		?? throw new ArgumentNullException(nameof(tokenRequester));
	private readonly TokenIssuerSettings _tokenIssuerSettings = tokenIssuerSettings.Value
		?? throw new ArgumentNullException(nameof(tokenIssuerSettings));
	private readonly IntegrationHttpSettings _integrationSettings = integrationSettings.Value
		?? throw new ArgumentNullException(nameof(integrationSettings));

    public async Task<IntegrationHttpResponse> PostAsync(string path, object request)
    {
        var response = await _httpRequester.PostAsync<IntegrationHttpResponse>(
            $"{_integrationSettings.ApiGatewayBaseUrl}/{path}",
            request,
            await GetAccessToken());

        return response;
    }

    public async Task<IntegrationHttpResponse> PutAsync(string path, object? request = null)
    {
        var response = await _httpRequester.PutAsync<IntegrationHttpResponse>(
            $"{_integrationSettings.ApiGatewayBaseUrl}/{path}",
            request,
            await GetAccessToken());
        return response;
    }

    public async Task<IntegrationHttpResponse> DeleteAsync(string path, object? request = null)
    {       
        var response = await _httpRequester.DeleteAsync<IntegrationHttpResponse>(
            $"{_integrationSettings.ApiGatewayBaseUrl}/{path}",
            request,
            await GetAccessToken());

        return response;
    }

    public async Task<IntegrationHttpResponse<TResponse>> FilterAsync<TResponse>(string path, object request)
        where TResponse : class
    {
        var response = await _httpRequester.PostAsync<IntegrationHttpResponse<TResponse>>(
            $"{_integrationSettings.ApiGatewayBaseUrl}/{path}",
            request,
            await GetAccessToken());

        return response;
    }

    public async Task<IntegrationHttpResponse<TResponse>> GetAsync<TResponse>(string path)
        where TResponse : class
    {
        var response = await _httpRequester.GetAsync<IntegrationHttpResponse<TResponse>>(
            $"{_integrationSettings.ApiGatewayBaseUrl}/{path}",
            await GetAccessToken());

        return response;
    }

    private async Task<string> GetAccessToken()
    {
        TokenResponse tokenResponse = await _tokenRequester
            .GetApplicationTokenAsync(_tokenIssuerSettings);
        return tokenResponse?.AccessToken ?? string.Empty;
    }
}
