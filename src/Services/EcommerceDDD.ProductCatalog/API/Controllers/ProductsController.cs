namespace EcommerceDDD.ProductCatalog.API.Controllers;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController(
	ICommandBus commandBus,
	IQueryBus queryBus
) : CustomControllerBase(commandBus, queryBus)
{
    [HttpPost]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer + "," + Roles.M2MAccess, Policy = Policies.CanRead)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<ProductViewModel?>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListProducts([FromBody] GetProductsRequest request,
        CancellationToken cancellationToken) =>  
			await Response(
				GetProducts.Create(
					request.CurrencyCode,
					ProductId.Of(request.ProductIds).ToList()
				),
				cancellationToken
			);
}