using MediatR;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Quotes.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Quotes.API.Controllers.Requests;
using EcommerceDDD.Quotes.Application.GettingOpenQuote;
using EcommerceDDD.Quotes.Application.GettingQuoteHistory;
using EcommerceDDD.Quotes.Api.Application.GettingQuoteHistory;
using EcommerceDDD.Quotes.Application.AddingQuoteItem;
using EcommerceDDD.Application.Quotes.RemovingQuoteItem;
using EcommerceDDD.Application.Quotes.CancelingQuote;
using EcommerceDDD.Quotes.Application.PlacingOrderFromQuote;
using EcommerceDDD.Quotes.Application.OpeningQuote;

namespace EcommerceDDD.Quotes.API.Controllers;

[Authorize]
[Route("api/quotes")]
[ApiController]
public class QuotesController : CustomControllerBase
{
    public QuotesController(IMediator mediator)
        : base(mediator) { }

    /// <summary>
    /// Get customer's open quote
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}/quote/{currencyCode}")]
    [ProducesResponseType(typeof(QuoteDetails), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOpenQuote([FromRoute] Guid customerId, [FromRoute] string currencyCode)
    {
        var query = new GetOpenQuote(customerId, currencyCode);
        return await Response(query);
    }

    /// <summary>
    /// Get quote event history
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpGet, Route("{quoteId}/history")]
    [ProducesResponseType(typeof(IList<QuoteEventHistory>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEventHistory([FromRoute] Guid quoteId)
    {
        var query = new GetQuoteEventHistory(quoteId);
        return await Response(query);
    }

    /// <summary>
    /// Open a Quote 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OpenQuote([FromBody] OpenQuoteRequest request)
    {
        var command = new OpenQuote(CustomerId.Of(request.CustomerId));
        return await Response(command);

    }

    /// <summary>
    /// Add a Quote item
    /// </summary>
    /// <param name="quoteId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut, Route("{quoteId:guid}/items")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddQuoteItem([FromRoute] Guid quoteId, [FromBody] AddQuoteItemRequest request)
    {
        var command = new AddQuoteItem(
            QuoteId.Of(quoteId),
            ProductId.Of(request.ProductId), 
            request.Quantity,
            request.CurrencyCode);

        return await Response(command);
    }

    /// <summary>
    /// Delete a Quote item
    /// </summary>
    /// <param name="quoteId"></param>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpDelete, Route("{quoteId:guid}/items/{productId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveQuoteItem([FromRoute] Guid quoteId, [FromRoute] Guid productId)
    {
        var command = new RemoveQuoteItem(QuoteId.Of(quoteId), ProductId.Of(productId));
        return await Response(command);
    }

    /// <summary>
    /// Cancel a Quote
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpDelete, Route("{quoteId:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelQuote([FromRoute] Guid quoteId)
    {
        var command = new CancelQuote(QuoteId.Of(quoteId));
        return await Response(command);
    }

    /// <summary>
    /// Place an order from a quote
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpPut, Route("{quoteId:guid}/placeorder/{currencyCode}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrder([FromRoute] Guid quoteId, [FromRoute] string currencyCode)
    {        
        var command = new PlaceOrderFromQuote(QuoteId.Of(quoteId), currencyCode);
        return await Response(command);
    }
}