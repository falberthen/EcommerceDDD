namespace EcommerceDDD.PaymentProcessing.Domain;

public interface ICustomerCreditChecker
{
    Task<bool> CheckIfCreditIsEnoughAsync(CustomerId customerId, Money totalAmount, CancellationToken cancellationToken);
}