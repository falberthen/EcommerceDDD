namespace EcommerceDDD.OrderProcessing.Application.Payments.CancelingPayment;

public class RequestCancelPaymentHandler : ICommandHandler<RequestCancelPayment>
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IConfiguration _configuration;

    public RequestCancelPaymentHandler(
        IIntegrationHttpService integrationHttpService,
        IConfiguration configuration)
    {
        _integrationHttpService = integrationHttpService;
        _configuration = configuration;
    }

    public async Task Handle(RequestCancelPayment command, CancellationToken cancellationToken)
    {
        var apiRoute = _configuration["ApiRoutes:PaymentProcessing"];
        var response = await _integrationHttpService.DeleteAsync(
            $"{apiRoute}/{command.PaymentId.Value}",
            new CancelPaymentRequest((int)command.PaymentCancellationReason));

        if (response?.Success == false)
            throw new ApplicationLogicException($"An error occurred requesting cancelling payment {command.PaymentId.Value}.");
    }
}

public record class CancelPaymentRequest(int PaymentCancellationReason);