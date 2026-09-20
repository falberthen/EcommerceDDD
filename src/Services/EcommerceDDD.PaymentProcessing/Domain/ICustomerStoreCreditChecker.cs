namespace EcommerceDDD.PaymentProcessing.Domain;

public interface ICustomerStoreCreditChecker
{
    Task<bool> CheckIfStoreCreditIsEnoughAsync(CustomerId customerId, Money totalAmount, CancellationToken cancellationToken);
}