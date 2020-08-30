using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ontactBookCQRS.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Application.Orders.PlaceOrder;
using EcommerceDDD.Infrastructure.Identity.Helpers;
using EcommerceDDD.Application.Orders.ChangeOrder;

namespace EcommerceDDD.WebApp.Controllers
{
    [Authorize]
    [Route("api/customers")]
    [ApiController]
    public class OrdersController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public OrdersController(
            IMediator mediator,
            IUserProvider userProvider,
            IMapper mapper)
            : base(userProvider)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost, Route("{customerId:guid}/orders")]
        [Authorize(Policy = "CanSave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PlaceOrder([FromRoute]Guid customerId, [FromBody] PlaceOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var command = new PlaceOrderCommand(customerId, request.Products, request.Currency);
            return Response(await _mediator.Send(command));
        }

        [HttpPut, Route("{customerId:guid}/orders/{orderId:guid}")]
        [Authorize(Policy = "CanSave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeOrder([FromRoute] Guid customerId, [FromRoute] Guid orderId, 
            [FromBody] ChangeOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var command = new ChangeOrderCommand(customerId, orderId, request.Products, request.Currency);
            return Response(await _mediator.Send(command));
        }
    }
}