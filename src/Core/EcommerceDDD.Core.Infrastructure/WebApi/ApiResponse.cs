namespace EcommerceDDD.Core.Infrastructure.WebApi;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}