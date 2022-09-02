using MediatR;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Orders.Application.Quotes;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.IntegrationServices.Orders.Requests;
using EcommerceDDD.Orders.Application.Orders.GettingOrders;
using EcommerceDDD.Orders.Application.GettingOrderEventHistory;
using EcommerceDDD.Orders.Application.Orders.PlacingOrder;

namespace EcommerceDDD.Orders.API.Controllers;

[Authorize]
[Route("api/orders")]
[ApiController]
public class OrdersController : CustomControllerBase
{
    public OrdersController(IMediator mediator) 
        : base(mediator) {}

    /// <summary>
    /// Get customer's orders
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}")]
    [ProducesResponseType(typeof(List<OrderDetails>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrders([FromRoute] Guid customerId)
    {
        var query = new GetOrders(customerId);
        return await Response(query);
    }

    /// <summary>
    /// Get order event history
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet, Route("{orderId}/history")]
    [ProducesResponseType(typeof(IList<OrderEventHistory>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEventHistory([FromRoute] Guid orderId)
    {
        var query = new GetOrderEventHistory(orderId);
        return await Response(query);
    }

    /// <summary>
    /// Places an order
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost, Route("{orderId}")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrder([FromRoute] Guid orderId, [FromBody] PlaceOrderRequest request)
    {
        var confirmedQuote = ConfirmedQuote.FromRequest(request);
        var command = new PlaceOrder(OrderId.Of(orderId), confirmedQuote);
        return await Response(command);
    }
}
