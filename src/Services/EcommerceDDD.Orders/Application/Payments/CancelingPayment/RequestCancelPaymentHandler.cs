using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.IntegrationServices;
using EcommerceDDD.IntegrationServices.Payments;
using EcommerceDDD.IntegrationServices.Payments.Requests;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.Orders.Application.Payments.CancelingPayment;

public class RequestCancelPaymentHandler : CommandHandler<RequestCancelPayment>
{
    private readonly IPaymentsService _paymentsService;
    private readonly IntegrationServicesSettings _integrationServicesSettings;

    public RequestCancelPaymentHandler(
        IPaymentsService paymentService,
        IOptions<IntegrationServicesSettings> integrationServicesSettings)
    {
        if (integrationServicesSettings == null)
            throw new ArgumentNullException(nameof(integrationServicesSettings));

        _paymentsService = paymentService;
        _integrationServicesSettings = integrationServicesSettings.Value;
    }

    public override async Task Handle(RequestCancelPayment command, CancellationToken cancellationToken)
    {
        await _paymentsService.CancelPayment(
            _integrationServicesSettings.ApiGatewayBaseUrl,
            command.PaymentId.Value,
            new CancelPaymentRequest((int)command.PaymentCancellationReason));
    }
}