namespace EcommerceDDD.ProductCatalog.API.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : CustomControllerBase
{
    public ProductsController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    [HttpPost]
    [Authorize(Policy = Policies.CanRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<ProductViewModel>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListProducts([FromBody] GetProductsRequest request,
        CancellationToken cancellationToken) =>  
        await Response(
            GetProducts.Create(request.CurrencyCode,
            ProductId.Of(request.ProductIds).ToList()),
            cancellationToken
        );
}