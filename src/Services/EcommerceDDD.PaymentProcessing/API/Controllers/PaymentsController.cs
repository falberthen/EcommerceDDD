using EcommerceDDD.PaymentProcessing.Application.CancelingPayment;

namespace EcommerceDDD.PaymentProcessing.API.Controllers;

[Authorize(Roles = Roles.M2MAccess)]
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/payments")]
public class PaymentsController(
	ICommandBus commandBus,
	IQueryBus queryBus
) : CustomControllerBase(commandBus, queryBus)
{ 
	[HttpPost]
	[MapToApiVersion(ApiVersions.V2)]
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
					Currency.OfCode(request.CurrencyCode),
					request.ProductItems.Select(p =>
						new ProductItem(
							ProductId.Of(p.ProductId),
							p.Quantity)
						).ToList()
				),
				cancellationToken
			);
    
    [HttpDelete("{paymentId:guid}")]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Policy = Policies.CanDelete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel([FromRoute] Guid paymentId, [FromBody] CancelPaymentRequest request, 
		CancellationToken cancellationToken) =>
			await Response(
				CancelPayment.Create(
					PaymentId.Of(paymentId),
					request.PaymentCancellationReason
				),
				cancellationToken
			);    
}
