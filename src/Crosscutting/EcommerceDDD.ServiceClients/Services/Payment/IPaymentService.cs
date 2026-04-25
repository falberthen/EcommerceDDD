namespace EcommerceDDD.ServiceClients.Services.Payment;

public interface IPaymentService
{
    Task RequestPaymentAsync(Guid customerId, Guid orderId, string currencyCode, double totalAmount, IList<PaymentProductItem> items, CancellationToken cancellationToken);
    Task CancelPaymentAsync(Guid orderId, Guid paymentId, int cancellationReason, CancellationToken cancellationToken);
}
