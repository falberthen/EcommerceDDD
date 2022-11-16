namespace EcommerceDDD.Orders.Domain;

public enum OrderCancellationReason
{
    CanceledByUser = 1,
    ProductWasOutOfStock = 2,
    CustomerReachedCreditLimit = 3,
    PaymentFailed = 4
}