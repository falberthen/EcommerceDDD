namespace EcommerceDDD.Core.Infrastructure.Http;

public interface IHttpRequester
{
    Task<T> GetAsync<T>(string url, string bearerToken = null) where T : class;
    Task<T> PostAsync<T>(string url, object body, string bearerToken = null) where T : class;
    Task<T> PutAsync<T>(string url, object body, string bearerToken = null) where T : class;
    Task<T> DeleteAsync<T>(string url, object body, string bearerToken = null) where T : class;
}
