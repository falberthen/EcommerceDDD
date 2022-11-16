using System.Net;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Quotes.Domain;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.Quotes.Domain.Commands;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Quotes.API.Controllers.Requests;
using EcommerceDDD.Quotes.Application.Quotes.GettingOpenQuote;
using EcommerceDDD.Quotes.Application.Quotes.GettingQuoteHistory;
using EcommerceDDD.Quotes.Application.Quotes.GettingConfirmedQuote;

namespace EcommerceDDD.Quotes.API.Controllers;

[Authorize]
[Route("api/quotes")]
[ApiController]
public class QuotesController : CustomControllerBase
{
    public QuotesController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    /// <summary>
    /// Get the current customer's quote
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}/quote/{currencyCode}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerOpenQuote([FromRoute] Guid customerId, [FromRoute] string currencyCode)
    {
        var query = GetOpenQuote
            .Create(CustomerId.Of(customerId), currencyCode);
        return await Response(query);
    }

    [HttpGet, Route("{quoteId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromRoute] Guid quoteId)
    {
        var query = GetConfirmedQuoteById.Create(QuoteId.Of(quoteId));
        return await Response(query);
    }

    /// <summary>
    /// Get quote event history
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpGet, Route("{quoteId}/history")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid quoteId)
    {
        var query = GetQuoteEventHistory.Create(QuoteId.Of(quoteId));
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
    public async Task<IActionResult> Create([FromBody] OpenQuoteRequest request)
    {
        var command = OpenQuote.Create(CustomerId.Of(request.CustomerId));
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
    public async Task<IActionResult> AddItem([FromRoute] Guid quoteId, [FromBody] AddQuoteItemRequest request)
    {
        var command = AddQuoteItem.Create(
            QuoteId.Of(quoteId),
            ProductId.Of(request.ProductId),
            request.Quantity,
            Currency.OfCode(request.CurrencyCode));

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
    public async Task<IActionResult> RemoveItem([FromRoute] Guid quoteId, [FromRoute] Guid productId)
    {
        var command = RemoveQuoteItem.Create(
            QuoteId.Of(quoteId), ProductId.Of(productId));

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
    public async Task<IActionResult> Cancel([FromRoute] Guid quoteId)
    {
        var command = CancelQuote.Create(QuoteId.Of(quoteId));
        return await Response(command);
    }

    /// <summary>
    /// Confirms the quote
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpPut, Route("{quoteId:guid}/confirm/{currencyCode}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirm([FromRoute] Guid quoteId, [FromRoute] string currencyCode)
    {
        var command = ConfirmQuote.Create(
            QuoteId.Of(quoteId), Currency.OfCode(currencyCode));

        return await Response(command);
    }
}