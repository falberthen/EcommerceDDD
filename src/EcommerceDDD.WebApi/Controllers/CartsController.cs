using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Infrastructure.Identity.Helpers;
using EcommerceDDD.Application.Carts.GetCartDetails;
using EcommerceDDD.Application.Carts.CreateCart;
using MediatR;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.WebApi.Controllers.Base;

namespace EcommerceDDD.WebApi.Controllers
{
    [Authorize]
    [Route("api/carts")]
    [ApiController]
    public class CartsController : BaseController
    {
        public CartsController(
            IMediator mediator,
            IUserProvider userProvider)
            : base(userProvider, mediator)
        {
        }

        /// <summary>
        /// Add or Change a cart 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "CanSave")]
        [ProducesResponseType(typeof(CommandHandlerResult<Guid>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveCart([FromBody] SaveCartRequest request)
        {
            var command = new SaveCartCommand(request.CustomerId, request.Product);
            return await Response(command);
        }

        /// <summary>
        /// Returns the cart details
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [HttpGet, Route("{customerId:guid}/details/{currency}")]
        [Authorize(Policy = "CanRead")]
        [ProducesResponseType(typeof(List<CartDetailsViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCartDetails([FromRoute] Guid customerId, [FromRoute] string currency)
        {
            var query = new GetCartDetailsQuery(customerId, currency);
            return await Response(query);
        }
    }
}