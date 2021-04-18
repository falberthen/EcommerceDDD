using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Infrastructure.Identity.Helpers;
using EcommerceDDD.Application.Products.ListProducts;
using System.Collections.Generic;
using EcommerceDDD.Application.Customers.ViewModels;
using System.Net;
using EcommerceDDD.WebApp.Controllers.Base;

namespace EcommerceDDD.WebApp.Controllers
{
    [Authorize]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : BaseController
    {
        public ProductsController(
            IMediator mediator, 
            IUserProvider userProvider)
            : base(userProvider, mediator)
        {
        }

        [HttpGet, Route("{currency}")]
        [Authorize(Policy = "CanRead")]
        [ProducesResponseType(typeof(IList<ProductViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProducts([FromRoute]string currency)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var query = new ListProductsQuery(currency);
            return Response(await Mediator.Send(query));
        }
    }
}