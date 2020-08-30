using System;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceDDD.Infrastructure.Identity.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ontactBookCQRS.WebApp.Controllers.Base;
using EcommerceDDD.Application.Customers.RegisterCustomer;
using Microsoft.AspNetCore.Http;
using EcommerceDDD.Application.Customers.UpdateCustomer;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Application.Customers.ListCustomerEventHistory;
using EcommerceDDD.Application.Customers.AuthenticateCustomer;

namespace EcommerceDDD.WebApp.Controllers
{
    [Authorize]
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CustomersController(
            IMediator mediator,
            IUserProvider userProvider,
            IMapper mapper)
            : base(userProvider)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost, Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DoLogin([FromBody] AuthenticateCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var query = new AuthenticateCustomerQuery(request.Email, request.Password);
            return Response(await _mediator.Send(query));
        }

        [AllowAnonymous]
        [HttpPost, Route("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody]RegisterCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var command = _mapper.Map<RegisterCustomerCommand>(request);
            return Response(await _mediator.Send(command));
        }

        [HttpPut, Route("{customerId:guid}")]
        [Authorize(Policy = "CanSave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            request.Id = id;
            var command = _mapper.Map<UpdateCustomerCommand>(request);
            return Response(await _mediator.Send(command));
        }

        [HttpGet, Route("{customerId:guid}/eventhistory")]
        [Authorize(Policy = "CanRead")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ListEventHistory([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var query = new ListCustomerEventHistoryQuery(id);
            return Response(await _mediator.Send(query));
        }
    }
}