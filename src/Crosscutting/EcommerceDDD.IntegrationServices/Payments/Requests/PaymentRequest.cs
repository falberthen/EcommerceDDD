namespace EcommerceDDD.IntegrationServices.Payments.Requests;

public record class PaymentRequest(
    Guid CustomerId,
    Guid OrderId,
    decimal TotalAmount,
    string currencyCode);
