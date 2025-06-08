using EcommerceDDD.InventoryManagement.Application.DecreasingQuantityInStock;

namespace EcommerceDDD.InventoryManagement.API.Controllers;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/inventory")]
public class InventoryController(
	ICommandBus commandBus,
	IQueryBus queryBus
) : CustomControllerBase(commandBus, queryBus)
{
	/// <summary>
	/// Checks the quantity of a given stock unit in stock
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpPost("check-stock-quantity")]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.M2MAccess)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<InventoryStockUnitViewModel>>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> CheckStockQuantity([FromBody] CheckProductsInStockRequest request,
		CancellationToken cancellationToken) =>
			await Response(CheckProductsInStock.Create(
					ProductId.Of(request.ProductIds).ToList()
				),
				cancellationToken
			);

	/// <summary>
	/// Get inventory stock unit event history
	/// </summary>
	/// <param name="productId"></param>
	/// <returns></returns>
	[HttpGet, Route("{productId:guid}/history")]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<InventoryStockUnitEventHistory>>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ListHistory([FromRoute] Guid productId, 
		CancellationToken cancellationToken) =>
			await Response(
				GetInventoryStockUnitEventHistory
					.Create(ProductId.Of(productId)),
				cancellationToken
			);

	/// <summary>
	/// Decreases the quantity of a given stock unit | M2M only
	/// </summary>
	/// <param name="productId"></param>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpPut("{productId:guid}/decrease-stock-quantity")]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.M2MAccess, Policy = Policies.CanWrite)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> DecreaseQuantity([FromRoute] Guid productId,
		[FromBody] DecreaseQuantityInStockRequest request, 
		CancellationToken cancellationToken) =>
			await Response(
				DecreaseStockQuantity.Create(
					ProductId.Of(productId), request.DecreasedQuantity
				),
				cancellationToken
			);
}