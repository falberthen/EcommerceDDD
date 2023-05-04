namespace EcommerceDDD.Products.API.Controllers;

[Authorize]
[Route("api/products")]
[ApiController]
public class ProductsController : CustomControllerBase
{
    public ProductsController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    [HttpPost]
    [Authorize(Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<ProductViewModel>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListProducts([FromBody] GetProductsRequest request) =>  
        await Response(
            GetProducts.Create(request.CurrencyCode,
            ProductId.Of(request.ProductIds).ToList())
        );    

    [HttpPost("stockavailability")]
    [Authorize(Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<ProductInStockViewModel>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckStockAvailability([FromBody] ProductStockAvailabilityRequest request) =>    
        await Response(
            CheckProductStockAvailability.Create(
            ProductId.Of(request.ProductIds).ToList())
        );
}