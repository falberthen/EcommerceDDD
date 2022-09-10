namespace EcommerceDDD.Customers.Application.GettingAvailableCreditLimit;

public record class AvailableCreditLimitModel(Guid CustomerId, decimal AvailableCreditLimit);

