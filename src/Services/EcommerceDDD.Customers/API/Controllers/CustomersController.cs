﻿using MediatR;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Customers.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Customers.API.Controllers.Requests;
using EcommerceDDD.Customers.Application.GettingCustomerDetails;
using EcommerceDDD.Customers.Api.Application.GettingCustomerDetails;
using EcommerceDDD.Customers.Application.GettingCustomerEventHistory;
using EcommerceDDD.Customers.Api.Application.RegisteringCustomer;
using EcommerceDDD.Customers.Api.Application.UpdatingCustomerInformation;

namespace EcommerceDDD.Customers.API.Controllers;

[Route("api/customers")]
[ApiController]
public class CustomersController : CustomControllerBase
{
    public CustomersController(IMediator mediator) 
        : base(mediator) { }

    /// <summary>
    /// Get customer details
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(CustomerDetails), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDetails()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var query = new GetCustomerDetails(accessToken!);
        return await Response(query);
    }

    /// <summary>
    /// Get customer event history
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet, Route("{customerId}/history")]
    [ProducesResponseType(typeof(IList<CustomerEventHistory>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEventHistory([FromRoute] Guid customerId)
    {
        var query = new GetCustomerEventHistory(new CustomerId(customerId));
        return await Response(query);
    }

    /// <summary>
    /// Register a new customer
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCustomerRequest request)
    {
        var command = new RegisterCustomer(
            request.Email,
            request.Password,
            request.PasswordConfirm,
            request.Name,
            request.Address);

        return await Response(command);
    }

    /// <summary>
    /// Update customer's information
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut, Route("{customerId}")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] Guid customerId, [FromBody] UpdateCustomerRequest request)
    {
        var command = new UpdateCustomerInformation(
            new CustomerId(customerId),
            request.Name,
            request.Address);

        return await Response(command);
    }
}