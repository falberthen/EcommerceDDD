namespace EcommerceDDD.Payments.Domain;

public interface ICustomerCreditChecker
{
    Task<bool> EnsureEnoughCredit(CustomerId customerId, Money totalAmount);
}