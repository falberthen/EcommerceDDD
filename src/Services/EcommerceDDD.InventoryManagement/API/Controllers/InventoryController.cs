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
	/// Get inventory stock unit event history
	/// </summary>
	/// <param name="productId"></param>
	/// <returns></returns>
	[HttpGet, Route("{productId:guid}/history")]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
	[ProducesResponseType(typeof(IReadOnlyList<InventoryStockUnitEventHistory>), StatusCodes.Status200OK)]
	public async Task<IActionResult> ListHistory([FromRoute] Guid productId,
		CancellationToken cancellationToken) =>
		await Response(
			GetInventoryStockUnitEventHistory.Create(ProductId.Of(productId)),
			cancellationToken
		);

}
