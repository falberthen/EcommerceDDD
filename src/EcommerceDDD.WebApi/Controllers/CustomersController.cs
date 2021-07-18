using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Application.Customers.RegisterCustomer;
using Microsoft.AspNetCore.Http;
using EcommerceDDD.Application.Customers.UpdateCustomer;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Application.Customers.AuthenticateCustomer;
using EcommerceDDD.Application.Customers.ListCustomerStoredEvents;
using System.Net;
using EcommerceDDD.Application.Customers.ViewModels;
using System.Collections.Generic;
using BuildingBlocks.CQRS.CommandHandling;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Application.EventSourcing.StoredEventsData;
using EcommerceDDD.WebApi.Controllers.Base;

namespace EcommerceDDD.WebApi.Controllers
{
    [Authorize]
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : BaseController
    {
        private readonly IMapper _mapper;

        public CustomersController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Authenticates an user and returns JWT 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, Route("login")]
        [ProducesResponseType(typeof(CustomerViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DoLogin([FromBody]AuthenticateCustomerRequest request)
        {
            var query = new AuthenticateCustomerQuery(request.Email, request.Password);
            return await Response(query);
        }

        /// <summary>
        /// Register a new customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, Route("register")]
        [ProducesResponseType(typeof(CommandHandlerResult<Guid>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody]RegisterCustomerRequest request)
        {
            var command = _mapper.Map<RegisterCustomerCommand>(request);
            return await Response(command);
        }

        /// <summary>
        /// Update a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut, Route("{customerId:guid}")]
        [Authorize(Policy = "CanSave")]
        [ProducesResponseType(typeof(CommandHandlerResult<Guid>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute]Guid customerId, [FromBody]UpdateCustomerRequest request)
        {
            var command = new UpdateCustomerCommand(customerId, request.Name);
            return await Response(command);
        }

        /// <summary>
        /// Returns the Stored Events of a given customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet, Route("{customerId:guid}/events")]
        [Authorize(Policy = "CanRead")]
        [ProducesResponseType(typeof(QueryHandlerResult<IList<CustomerStoredEventData>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ListEvents([FromRoute]Guid customerId)
        {
            var query = new ListCustomerStoredEventsQuery(customerId);
            return await Response(query);
        }
    }
}