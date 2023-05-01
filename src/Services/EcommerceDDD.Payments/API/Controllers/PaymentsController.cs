namespace EcommerceDDD.Payments.API.Controllers;

[Authorize]
[Route("api/payments")]
[ApiController]
public class PaymentsController : CustomControllerBase
{
    public PaymentsController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    [HttpPost]
    [Authorize(Policy = PolicyBuilder.WritePolicy)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromBody] PaymentRequest request)
    {
        var command = RequestPayment.Create(
            CustomerId.Of(request.CustomerId),
            OrderId.Of(request.OrderId),
            Money.Of(request.TotalAmount, request.CurrencyCode),
            Currency.OfCode(request.CurrencyCode));

        return await Response(command);
    }

    [HttpDelete("{paymentId}")]
    [Authorize(Policy = PolicyBuilder.DeletePolicy)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> Cancel([FromRoute] Guid paymentId, [FromBody] CancelPaymentRequest request)
    {
        var command = CancelPayment.Create(
            PaymentId.Of(paymentId),
            (PaymentCancellationReason)request.PaymentCancellationReason);

        return await Response(command);
    }
}
