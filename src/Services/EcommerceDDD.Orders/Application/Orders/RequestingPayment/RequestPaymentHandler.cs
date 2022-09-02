using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.IntegrationServices;
using EcommerceDDD.IntegrationServices.Payments;
using EcommerceDDD.IntegrationServices.Payments.Requests;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.Orders.Application.Orders.RequestingPayment;

public class RequestPaymentHandler : CommandHandler<RequestPayment>
{
    private readonly IPaymentsService _paymentsService;
    private readonly IntegrationServicesSettings _integrationServicesSettings;

    public RequestPaymentHandler(
        IPaymentsService paymentService,
        IOptions<IntegrationServicesSettings> integrationServicesSettings)
    {
        if (integrationServicesSettings == null)
            throw new ArgumentNullException(nameof(integrationServicesSettings));

        _paymentsService = paymentService;
        _integrationServicesSettings = integrationServicesSettings.Value;
    }

    public override async Task Handle(RequestPayment command, CancellationToken cancellationToken)
    {
        await _paymentsService.RequestPayment(
            _integrationServicesSettings.ApiGatewayBaseUrl,
            new PaymentRequest(
                command.CustomerId.Value,
                command.OrderId.Value,
                command.TotalPrice.Value,
                command.CurrencyCode));
    }
}