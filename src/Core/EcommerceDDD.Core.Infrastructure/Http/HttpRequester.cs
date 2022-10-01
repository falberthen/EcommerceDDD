using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace EcommerceDDD.Core.Infrastructure.Http;

public class HttpRequester : IHttpRequester
{    
    private HttpClient _httpClient;
    private const string _scheme = "Bearer";

    public HttpRequester(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    public async Task<T?> PostAsync<T>(string url, object body, string? bearerToken = null) where T : class
    {
        if (!string.IsNullOrEmpty(bearerToken))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_scheme, bearerToken);

        var response = await _httpClient.PostAsync(url, SerializeBody(body));            
        return DeserializeResponse<T>(response);
    }

    public async Task<T?> PutAsync<T>(string url, object body, string? bearerToken = null) where T : class
    {
        if (!string.IsNullOrEmpty(bearerToken))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_scheme, bearerToken);

        var response = await _httpClient.PutAsync(url, SerializeBody(body));
        return DeserializeResponse<T>(response);
    }

    public async Task<T?> DeleteAsync<T>(string url, object? body = null, string? bearerToken = null) where T : class
    {
        if (!string.IsNullOrEmpty(bearerToken))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_scheme, bearerToken);

        var httpMessage = new HttpRequestMessage(HttpMethod.Delete, url);
        if (body is not null)
            httpMessage.Content = SerializeBody(body);
        
        var response = await _httpClient.SendAsync(httpMessage);
        return DeserializeResponse<T>(response);
    }

    public async Task<T?> GetAsync<T>(string url, string? bearerToken = null) where T : class
    {
        if (!string.IsNullOrEmpty(bearerToken))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_scheme, bearerToken);

        var response = await _httpClient.GetAsync(url);
        return DeserializeResponse<T>(response);
    }

    private StringContent SerializeBody(object body)
    {
        if (body is null)
            throw new ArgumentNullException("Body is required");

        var json = JsonConvert.SerializeObject(body);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private T? DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content.Result);
    }
}