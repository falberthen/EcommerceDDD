namespace EcommerceDDD.IntegrationServices.Base;

public record class IntegrationServiceResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}