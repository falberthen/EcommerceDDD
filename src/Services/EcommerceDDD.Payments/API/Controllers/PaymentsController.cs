namespace EcommerceDDD.Payments.API.Controllers;

[Authorize(Policy = PolicyBuilder.M2MPolicy)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestCreate([FromBody] PaymentRequest request) =>
        await Response(
            RequestPayment.Create(
            CustomerId.Of(request.CustomerId),
            OrderId.Of(request.OrderId),
            Money.Of(request.TotalAmount, request.CurrencyCode),
            Currency.OfCode(request.CurrencyCode))
        );
    
    [HttpDelete("{paymentId}")]
    [Authorize(Policy = PolicyBuilder.DeletePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel([FromRoute] Guid paymentId, [FromBody] CancelPaymentRequest request) =>    
        await Response(
            CancelPayment.Create(
            PaymentId.Of(paymentId),
            (PaymentCancellationReason)request.PaymentCancellationReason)
        );    
}
