namespace EcommerceDDD.Customers.Application.GettingCreditLimit;

public record class CreditLimitModel(
    Guid CustomerId,
    decimal CreditLimit);

