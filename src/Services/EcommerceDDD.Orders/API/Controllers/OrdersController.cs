using System.Net;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Orders.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Orders.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Orders.Application.Orders.GettingOrders;
using EcommerceDDD.Orders.Application.GettingOrderEventHistory;

namespace EcommerceDDD.Orders.API.Controllers;

[Authorize]
[Route("api/orders")]
[ApiController]
public class OrdersController : CustomControllerBase
{
    public OrdersController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    /// <summary>
    /// Get customer's orders
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListDetails([FromRoute] Guid customerId)
    {
        var query = GetOrders.Create(CustomerId.Of(customerId));
        return await Response(query);
    }

    /// <summary>
    /// Get order event history
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet, Route("{orderId}/history")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid orderId)
    {
        var query = GetOrderEventHistory.Create(OrderId.Of(orderId));
        return await Response(query);
    }

    /// <summary>
    /// Places an order from a quote
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost, Route("{quoteId}")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrderFromQuote([FromRoute] Guid quoteId)
    {
        var command = PlaceOrder.Create(QuoteId.Of(quoteId));
        return await Response(command);
    }
}