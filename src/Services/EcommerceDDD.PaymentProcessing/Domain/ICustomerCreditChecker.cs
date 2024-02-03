namespace EcommerceDDD.PaymentProcessing.Domain;

public interface ICustomerCreditChecker
{
    Task<bool> IsCreditEnough(CustomerId customerId, Money totalAmount);
}