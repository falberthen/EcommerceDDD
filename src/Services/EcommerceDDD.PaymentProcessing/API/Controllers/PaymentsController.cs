namespace EcommerceDDD.PaymentProcessing.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize(Policy = Policies.M2MAccess)]
public class PaymentsController : CustomControllerBase
{
    public PaymentsController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    [HttpPost]
    [Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestCreate([FromBody] PaymentRequest request,
        CancellationToken cancellationToken) =>
        await Response(
            RequestPayment.Create(
            CustomerId.Of(request.CustomerId),
            OrderId.Of(request.OrderId),
            Money.Of(request.TotalAmount, request.CurrencyCode),
            Currency.OfCode(request.CurrencyCode)),
            cancellationToken
        );
    
    [HttpDelete("{paymentId:guid}")]
    [Authorize(Policy = Policies.CanDelete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel([FromRoute] Guid paymentId, 
        [FromBody] CancelPaymentRequest request, CancellationToken cancellationToken) =>
        await Response(
            CancelPayment.Create(
            PaymentId.Of(paymentId),
            (PaymentCancellationReason)request.PaymentCancellationReason),
            cancellationToken
        );    
}
