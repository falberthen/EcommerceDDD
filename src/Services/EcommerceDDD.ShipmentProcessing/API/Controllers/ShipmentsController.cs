namespace EcommerceDDD.ShipmentProcessing.API.Controllers;

[ApiController]
[Route("api/shipments")]
[Authorize(Policy = Policies.M2MAccess)]
public class ShipmentsController : CustomControllerBase
{
    public ShipmentsController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    [HttpPost]
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