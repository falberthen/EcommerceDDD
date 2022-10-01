namespace EcommerceDDD.Payments.Domain;

public record class PaymentData(
    CustomerId CustomerId,
    OrderId OrderId,
    Money TotalAmount);