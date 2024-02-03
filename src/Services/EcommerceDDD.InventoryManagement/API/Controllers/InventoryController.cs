namespace EcommerceDDD.InventoryManagement.API.Controllers;

[Authorize]
[Route("api/inventory")]
[ApiController]
public class InventoryController : CustomControllerBase
{
    public InventoryController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    /// <summary>
    /// Checks the quantity of a given stock unit in stock
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("check-stock-quantity")]
    [Authorize(Policy = Policies.M2MAccess)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<InventoryStockUnitViewModel>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckStockQuantity([FromBody] CheckProductsInStockRequest request,
        CancellationToken cancellationToken) =>
        await Response(CheckProductsInStock.Create(
            ProductId.Of(request.ProductIds).ToList()),
            cancellationToken
        );

    /// <summary>
    /// Decreases the quantity of a given stock unit
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("decrease-stock-quantity")]
    [Authorize(Policy = Policies.M2MAccess)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<InventoryStockUnitViewModel>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DecreaseQuantity([FromBody] DecreaseQuantityInStockRequest request
        , CancellationToken cancellationToken) =>
        await Response(DecreaseStockQuantity.Create(
            ProductId.Of(request.ProductId), request.DecreasedQuantity),
            cancellationToken
        );

    /// <summary>
    /// Increases the quantity of a given stock unit
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("increase-stock-quantity")]
    [Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<InventoryStockUnitViewModel>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IncreaseQuantity([FromBody] IncreaseQuantityInStockRequest request,
        CancellationToken cancellationToken) =>
        await Response(IncreaseStockQuantity.Create(
            ProductId.Of(request.ProductId), request.IncreasedQuantity),
            cancellationToken
        );

    /// <summary>
    /// Get inventory stock unit event history
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpGet, Route("{productId:guid}/history")]
    [Authorize(Policy = Policies.CanRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<InventoryStockUnitEventHistory>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid productId,
        CancellationToken cancellationToken) =>
        await Response(
            GetInventoryStockUnitEventHistory
                .Create(ProductId.Of(productId)),
            cancellationToken
        );
}