namespace EcommerceDDD.Core.Infrastructure.Integration;

public class IntegrationHttpService : IIntegrationHttpService
{
    private readonly IHttpRequester _httpRequester;
    private readonly ITokenRequester _tokenRequester;
    private readonly TokenIssuerSettings _tokenIssuerSettings;
    private readonly IntegrationHttpSettings _integrationSettings;

    public IntegrationHttpService(
        IHttpRequester httpRequester,
        ITokenRequester tokenRequester,
        IOptions<TokenIssuerSettings> tokenIssuerSettings,
        IOptions<IntegrationHttpSettings> integrationSettings)
    {
        if (tokenIssuerSettings is null)
            throw new ArgumentNullException(nameof(tokenIssuerSettings));
        if (integrationSettings is null)
            throw new ArgumentNullException(nameof(integrationSettings));

        _httpRequester = httpRequester;
        _tokenRequester = tokenRequester;
        _tokenIssuerSettings = tokenIssuerSettings.Value;
        _integrationSettings = integrationSettings.Value;
    }

    public async Task<IntegrationHttpResponse> PostAsync(string path, object request)
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationTokenAsync(_tokenIssuerSettings);

        var response = await _httpRequester.PostAsync<IntegrationHttpResponse>(
            $"{_integrationSettings.ApiGatewayBaseUrl}/{path}",
            request,
            tokenResponse.AccessToken);

        return response;
    }

    public async Task<IntegrationHttpResponse> DeleteAsync(string path, object request)
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationTokenAsync(_tokenIssuerSettings);

        var response = await _httpRequester.DeleteAsync<IntegrationHttpResponse>(
            $"{_integrationSettings.ApiGatewayBaseUrl}/{path}",
            request,
            tokenResponse.AccessToken);

        return response;
    }

    public async Task<IntegrationHttpResponse<TResponse>> FilterAsync<TResponse>(string path, object request)
        where TResponse : class
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationTokenAsync(_tokenIssuerSettings);

        var response = await _httpRequester.PostAsync<IntegrationHttpResponse<TResponse>>(
            $"{_integrationSettings.ApiGatewayBaseUrl}/{path}",
            request,
            tokenResponse.AccessToken);

        return response;
    }

    public async Task<IntegrationHttpResponse<TResponse>> GetAsync<TResponse>(string path)
        where TResponse : class
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationTokenAsync(_tokenIssuerSettings);

        var response = await _httpRequester.GetAsync<IntegrationHttpResponse<TResponse>>(
            $"{_integrationSettings.ApiGatewayBaseUrl}/{path}",
            tokenResponse.AccessToken);

        return response;
    }
}
