using MediatR;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Products.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.IntegrationServices.Products.Requests;
using EcommerceDDD.Products.Application.Products.GettingProducts;
using EcommerceDDD.Products.Application.Products.CheckingProductStockAvailability;

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

    [HttpPost("stockavailability")]
    [ProducesResponseType(typeof(IList<ProductViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckProductStockAvailability([FromBody] ProductStockAvailabilityRequest request)
    {
        var query = new CheckProductStockAvailability(ProductId.Of(request.ProductIds).ToList());
        return await Response(query);
    }
}