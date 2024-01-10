namespace EcommerceDDD.Orders.Domain;

public enum OrderCancellationReason
{
    ProcessmentError = 0,
    CanceledByUser = 1,
    ProductWasOutOfStock = 2,
    CustomerReachedCreditLimit = 3,
    PaymentFailed = 4,
    ShipmentFailed = 4
}