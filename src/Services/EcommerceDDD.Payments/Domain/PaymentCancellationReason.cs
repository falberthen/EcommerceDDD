namespace EcommerceDDD.Payments.Domain;

public enum PaymentCancellationReason
{
    OrderCanceled = 1,
    CustomerReachedCreditLimit = 2,
    ProcessmentError = 3
}