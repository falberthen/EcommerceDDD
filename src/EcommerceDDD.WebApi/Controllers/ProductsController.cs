using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Application.Products.ListProducts;
using System.Collections.Generic;
using EcommerceDDD.Application.Customers.ViewModels;
using System.Net;
using EcommerceDDD.WebApi.Controllers.Base;

namespace EcommerceDDD.WebApi.Controllers
{
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
}