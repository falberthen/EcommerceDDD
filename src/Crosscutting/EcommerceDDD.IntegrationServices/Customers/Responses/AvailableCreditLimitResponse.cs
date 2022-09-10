namespace EcommerceDDD.IntegrationServices.Customers.Responses;

public record class AvailableCreditLimitResponse
{
    public bool Success { get; set; }
    public AvailableCreditLimitModel Data { get; set; }
}

public record class AvailableCreditLimitModel(Guid CustomerId, decimal AvailableCreditLimit);