using EcommerceDDD.Application.Products;
using EcommerceDDD.WebApi.Controllers.Base;
using EcommerceDDD.Application.Products.ListProducts;

namespace EcommerceDDD.WebApi.Controllers;

[Authorize]
[Route("api/products")]
[ApiController]
public class ProductsController : BaseController
{
    public ProductsController(
        IMediator mediator)
        : base(mediator)
    {
    }

    [HttpGet, Route("{currency}")]
    [Authorize(Policy = "CanRead")]
    [ProducesResponseType(typeof(IList<ProductViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts([FromRoute]string currency)
    {
        var query = new ListProductsQuery(currency);
        return await Response(query);
    }
}