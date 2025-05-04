namespace EcommerceDDD.ShipmentProcessing.API.Controllers;

[Authorize(Roles = Roles.M2MAccess)]
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/shipments")]
public class ShipmentsController(
	ICommandBus commandBus,
	IQueryBus queryBus
) : CustomControllerBase(commandBus, queryBus)
{
    [HttpPost]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestOrderShipment([FromBody] ShipOrderRequest request
        , CancellationToken cancellationToken) =>    
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