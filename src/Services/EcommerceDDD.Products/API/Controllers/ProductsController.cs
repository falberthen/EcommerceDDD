using MediatR;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Products.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Products.API.Controllers.Requests;
using EcommerceDDD.Products.Application.Products.GettingProducts;
using EcommerceDDD.Products.Application.Products.CheckingProductStockAvailability;

namespace EcommerceDDD.Products.API.Controllers;

[Authorize]
[Route("api/products")]
[ApiController]
public class ProductsController : CustomControllerBase
{
    public ProductsController(IMediator mediator)
        : base(mediator) {}

    [HttpPost]
    [ProducesResponseType(typeof(IList<ProductViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListProducts([FromBody] GetProductsRequest request)
    {        
        var query = GetProducts.Create(request.CurrencyCode, 
            ProductId.Of(request.ProductIds).ToList());

        return await Response(query);
    }

    [HttpPost("stockavailability")]
    [ProducesResponseType(typeof(IList<ProductInStockViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckStockAvailability([FromBody] ProductStockAvailabilityRequest request)
    {
        var query = CheckProductStockAvailability.Create(
            ProductId.Of(request.ProductIds).ToList());

        return await Response(query);
    }
}