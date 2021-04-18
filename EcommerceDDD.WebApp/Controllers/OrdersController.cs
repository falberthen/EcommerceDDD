using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Application.Orders.PlaceOrder;
using EcommerceDDD.Infrastructure.Identity.Helpers;
using EcommerceDDD.Application.Orders.GetOrderDetails;
using EcommerceDDD.Application.Orders.GetOrders;
using System.Collections.Generic;
using System.Net;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.WebApp.Controllers.Base;

namespace EcommerceDDD.WebApp.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : BaseController
    {
        public OrdersController(
            IMediator mediator,
            IUserProvider userProvider)
            : base(userProvider, mediator)
        {
        }

        /// <summary>
        /// Returns the orders placed by a given cursomer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet, Route("{customerId:guid}")]
        [Authorize(Policy = "CanRead")]
        [ProducesResponseType(typeof(List<OrderDetailsViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrders([FromRoute] Guid customerId)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var query = new GetOrdersQuery(customerId);
            return Response(await Mediator.Send(query));
        }

        /// <summary>
        /// Returns the details of a given order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet, Route("{orderId:guid}/details")]
        [Authorize(Policy = "CanRead")]
        [ProducesResponseType(typeof(OrderDetailsViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrderDetails([FromRoute] Guid orderId)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var query = new GetOrderDetailsQuery(orderId);
            return Response(await Mediator.Send(query));
        }

        /// <summary>
        /// Place an order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("{cartId:guid}")]
        [Authorize(Policy = "CanSave")]
        [ProducesResponseType(typeof(CommandHandlerResult<Guid>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PlaceOrder([FromRoute] Guid cartId, [FromBody] PlaceOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var command = new PlaceOrderCommand(cartId, request.CustomerId, request.Currency);                        
            return Response(await Mediator.Send(command));
        }
    }
}