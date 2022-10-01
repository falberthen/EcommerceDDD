namespace EcommerceDDD.Core.Infrastructure.Integration;

public class IntegrationHttpResponse<T>
    where T : class
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}

public class IntegrationHttpResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
}