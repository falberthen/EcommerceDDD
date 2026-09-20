namespace EcommerceDDD.PaymentProcessing.Domain;

public enum PaymentCancellationReason
{
    OrderCanceled = 1,
    CustomerReachedStoreCreditLimit = 2,
	ProductOutOfStock = 3,
}