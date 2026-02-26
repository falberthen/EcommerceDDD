using EcommerceDDD.ServiceClients.PaymentProcessing;
using EcommerceDDD.ServiceClients.PaymentProcessing.Models;

namespace EcommerceDDD.ServiceClients.Services.Payment;

public class PaymentService(PaymentProcessingClient paymentProcessingClient) : IPaymentService
{
    private readonly PaymentProcessingClient _paymentProcessingClient = paymentProcessingClient;

    public async Task RequestPaymentAsync(Guid customerId, Guid orderId, string currencyCode, double totalAmount, IList<PaymentProductItem> items, CancellationToken cancellationToken)
    {
        var productItems = items.Select(i => new ProductItemRequest()
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList();

        var request = new PaymentRequest()
        {
            CustomerId = customerId,
            OrderId = orderId,
            CurrencyCode = currencyCode,
            TotalAmount = totalAmount,
            ProductItems = productItems
        };

        await _paymentProcessingClient.Api.V2.Internal.Payments
            .PostAsync(request, cancellationToken: cancellationToken);
    }

    public async Task CancelPaymentAsync(Guid paymentId, int cancellationReason, CancellationToken cancellationToken)
    {
        var request = new CancelPaymentRequest()
        {
            PaymentCancellationReason = cancellationReason
        };

        await _paymentProcessingClient.Api.V2.Internal.Payments[paymentId]
            .DeleteAsync(request, cancellationToken: cancellationToken);
    }
}
