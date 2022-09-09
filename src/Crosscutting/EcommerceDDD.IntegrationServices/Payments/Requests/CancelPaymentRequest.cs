namespace EcommerceDDD.IntegrationServices.Payments.Requests;

public record class CancelPaymentRequest(int PaymentCancellationReason);
