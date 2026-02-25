namespace EcommerceDDD.ShipmentProcessing.API.Controllers;

[Authorize(Roles = Roles.M2MAccess)]
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/internal/shipments")]
public class ShipmentsController(
	ICommandBus commandBus,
	IQueryBus queryBus
) : CustomControllerBase(commandBus, queryBus)
{
	[HttpPost]
	[Authorize(Policy = Policies.CanWrite)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> RequestOrderShipment([FromBody] ShipOrderRequest request,
		CancellationToken cancellationToken) =>
		await Response(
			RequestShipment.Create(
				OrderId.Of(request.OrderId),
				request.ProductItems.Select(p =>
					new ProductItem(
						ProductId.Of(p.ProductId),
						p.Quantity)
				).ToList()
			), cancellationToken
		);
}
