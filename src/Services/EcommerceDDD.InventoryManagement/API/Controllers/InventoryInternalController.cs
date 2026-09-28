namespace EcommerceDDD.InventoryManagement.API.Controllers;

[Authorize(Roles = Roles.M2MAccess)] // M2M only
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/internal/inventory")]
public class InventoryInternalController(
	IMessageBus bus
) : CustomControllerBase(bus)
{
	/// <summary>
	/// Checks the quantity of a given stock unit in stock
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpPost("check-stock-quantity")]
	[Authorize(Policy = Policies.CanRead)]
	[ProducesResponseType(typeof(IList<InventoryStockUnitViewModel>), StatusCodes.Status200OK)]
	public async Task<IActionResult> CheckStockQuantity([FromBody] CheckProductsInStockRequest request,
		CancellationToken cancellationToken) =>
		await Response(
			CheckProductsInStock.Create(
				ProductId.Of(request.ProductIds).ToList()
			),
			cancellationToken
		);

	/// <summary>
	/// Decreases the quantity of a given stock unit (idempotent per order)
	/// </summary>
	/// <param name="productId"></param>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpPut("{productId:guid}/decrease-stock-quantity")]
	[Authorize(Policy = Policies.CanWrite)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> DecreaseQuantity([FromRoute] Guid productId,
		[FromBody] DecreaseQuantityInStockRequest request,
		CancellationToken cancellationToken) =>
		await Response(
			DecreaseStockQuantity.Create(
				ProductId.Of(productId), request.DecreasedQuantity, request.OrderId
			),
			cancellationToken
		);

	/// <summary>
	/// Returns to a given stock unit what an order took (idempotent per order)
	/// </summary>
	/// <param name="productId"></param>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpPut("{productId:guid}/increase-stock-quantity")]
	[Authorize(Policy = Policies.CanWrite)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> IncreaseQuantity([FromRoute] Guid productId,
		[FromBody] IncreaseQuantityInStockRequest request,
		CancellationToken cancellationToken) =>
		await Response(
			IncreaseStockQuantity.Create(
				ProductId.Of(productId), request.IncreasedQuantity, request.OrderId
			),
			cancellationToken
		);
}
