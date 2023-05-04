namespace EcommerceDDD.Shipments.API.Controllers;

[Authorize(Policy = PolicyBuilder.M2MPolicy)]
[Route("api/shipments")]
[ApiController]
public class ShipmentsController : CustomControllerBase
{
    public ShipmentsController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    [HttpPost]
    [Authorize(Policy = PolicyBuilder.WritePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestOrderShipment([FromBody] ShipOrderRequest request) =>    
        await Response(
            RequestShipment.Create(
                OrderId.Of(request.OrderId),
                request.ProductItems.Select(p =>
                    new ProductItem(
                        ProductId.Of(p.ProductId),
                        p.Quantity)
                ).ToList()
            )
        );
}