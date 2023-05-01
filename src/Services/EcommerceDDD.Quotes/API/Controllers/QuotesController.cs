namespace EcommerceDDD.Quotes.API.Controllers;

[Route("api/quotes")]
[ApiController]
public class QuotesController : CustomControllerBase
{
    public QuotesController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    /// <summary>
    /// Get a quote by Id
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpGet, Route("{quoteId}")]
    [Authorize(Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromRoute] Guid quoteId)
    {
        var query = GetConfirmedQuoteById.Create(QuoteId.Of(quoteId));
        return await Response(query);
    }

    /// <summary>
    /// Get the current customer's quote
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}/quote/{currencyCode}")]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerOpenQuote([FromRoute] Guid customerId, [FromRoute] string currencyCode)
    {
        var query = GetOpenQuote
            .Create(CustomerId.Of(customerId), currencyCode);
        return await Response(query);
    }

    /// <summary>
    /// Get quote event history
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpGet, Route("{quoteId}/history")]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.ReadPolicy)]
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
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.WritePolicy)]
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
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.WritePolicy)]
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
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.DeletePolicy)]
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
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.DeletePolicy)]
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
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.WritePolicy)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirm([FromRoute] Guid quoteId, [FromRoute] string currencyCode)
    {
        var command = ConfirmQuote.Create(
            QuoteId.Of(quoteId), Currency.OfCode(currencyCode));

        return await Response(command);
    }
}