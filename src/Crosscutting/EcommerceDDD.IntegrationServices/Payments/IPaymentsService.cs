using EcommerceDDD.IntegrationServices.Base;
using EcommerceDDD.IntegrationServices.Payments.Requests;

namespace EcommerceDDD.IntegrationServices.Payments;

public interface IPaymentsService
{
    Task<IntegrationServiceResponse> RequestPayment(string apiGatewayUrl, PaymentRequest request);
    Task<IntegrationServiceResponse> CancelPayment(string apiGatewayUrl, Guid paymentId, CancelPaymentRequest request);
}
