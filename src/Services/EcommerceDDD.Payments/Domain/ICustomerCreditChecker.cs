namespace EcommerceDDD.Payments.Domain;

public interface ICustomerCreditChecker
{
    Task<bool> IsCreditEnough(CustomerId customerId, Money totalAmount);
}