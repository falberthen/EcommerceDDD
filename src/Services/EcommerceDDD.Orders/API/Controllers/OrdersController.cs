using MediatR;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Orders.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Orders.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Orders.API.Controllers.Requests;
using EcommerceDDD.Orders.Infrastructure.Projections;
using EcommerceDDD.Orders.Application.Orders.GettingOrders;
using EcommerceDDD.Orders.Application.GettingOrderEventHistory;

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
    [ProducesResponseType(typeof(IList<OrderEventHistory>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid orderId)
    {
        var query = GetOrderEventHistory.Create(OrderId.Of(orderId));
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
    public async Task<IActionResult> CreateOrder([FromRoute] Guid orderId, [FromBody] PlaceOrderRequest request)
    {
        var items = request.Items.Select(qi =>
            new ProductItemData()
            {
                ProductId = ProductId.Of(qi.ProductId),
                ProductName = string.Empty,
                Quantity = qi.Quantity
            }).ToList();

        var orderData = new OrderData(
            OrderId.Of(orderId),
            QuoteId.Of(request.QuoteId),
            CustomerId.Of(request.CustomerId),
            items,
            Currency.OfCode(request.CurrencyCode));

        var command = PlaceOrder.Create(orderData);
        return await Response(command);
    }
}