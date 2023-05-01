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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListProducts([FromBody] GetProductsRequest request)
    {
        var query = GetProducts.Create(request.CurrencyCode,
            ProductId.Of(request.ProductIds).ToList());

        return await Response(query);
    }

    [HttpPost("stockavailability")]
    [Authorize(Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckStockAvailability([FromBody] ProductStockAvailabilityRequest request)
    {
        var query = CheckProductStockAvailability.Create(
            ProductId.Of(request.ProductIds).ToList());

        return await Response(query);
    }
}