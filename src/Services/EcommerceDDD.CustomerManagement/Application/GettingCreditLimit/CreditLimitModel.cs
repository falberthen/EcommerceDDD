namespace EcommerceDDD.CustomerManagement.Application.GettingCreditLimit;

public record class CreditLimitModel(
    Guid CustomerId,
    decimal CreditLimit);

