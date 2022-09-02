using MediatR;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Products.Domain;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Products.Application.Products.GettingProducts;
using EcommerceDDD.IntegrationServices.Products.Requests;

namespace EcommerceDDD.Products.API.Controllers;

[Authorize]
[Route("api/products")]
[ApiController]
public class ProductsController : CustomControllerBase
{
    public ProductsController(IMediator mediator)
        : base(mediator) { }

    [HttpPost]
    [ProducesResponseType(typeof(IList<ProductViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts([FromBody] GetProductsRequest request)
    {        
        var query = new GetProducts(request.CurrencyCode, ProductId.Of(request.ProductIds).ToList());
        return await Response(query);
    }
}