namespace EcommerceDDD.Orders.Application.Payments.CancelingPayment;

public class RequestCancelPaymentHandler : ICommandHandler<RequestCancelPayment>
{
    private readonly IIntegrationHttpService _integrationHttpService;

    public RequestCancelPaymentHandler(IIntegrationHttpService integrationHttpService)
    {
        _integrationHttpService = integrationHttpService;     
    }

    public async Task Handle(RequestCancelPayment command, CancellationToken cancellationToken)
    {
        var response = await _integrationHttpService.DeleteAsync(
            $"api/payments/{command.PaymentId.Value}",
            new CancelPaymentRequest((int)command.PaymentCancellationReason));

        if (response is null || !response!.Success)
            throw new ApplicationLogicException($"An error occurred requesting cancelling payment {command.PaymentId.Value}.");
    }
}

public record class CancelPaymentRequest(int PaymentCancellationReason);