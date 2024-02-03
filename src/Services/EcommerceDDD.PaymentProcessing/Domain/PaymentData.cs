namespace EcommerceDDD.PaymentProcessing.Domain;

public record class PaymentData(
    CustomerId CustomerId,
    OrderId OrderId,
    Money TotalAmount);