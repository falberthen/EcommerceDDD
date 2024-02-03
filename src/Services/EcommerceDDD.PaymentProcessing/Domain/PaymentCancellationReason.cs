namespace EcommerceDDD.PaymentProcessing.Domain;

public enum PaymentCancellationReason
{
    ProcessmentError = 0,
    OrderCanceled = 1,
    CustomerReachedCreditLimit = 2
}