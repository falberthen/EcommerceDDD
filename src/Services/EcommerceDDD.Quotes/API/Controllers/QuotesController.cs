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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<QuoteViewModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromRoute] Guid quoteId) =>
        await Response(
            GetConfirmedQuoteById.Create(QuoteId.Of(quoteId))
        );

    /// <summary>
    /// Get the current customer's quote
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="currencyCode"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}/quote/{currencyCode}")]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<QuoteViewModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerOpenQuote([FromRoute] Guid customerId, [FromRoute] string currencyCode) =>    
        await Response(
           GetOpenQuote.Create(CustomerId.Of(customerId), currencyCode)
       );    

    /// <summary>
    /// Get quote event history
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpGet, Route("{quoteId}/history")]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<QuoteEventHistory>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid quoteId) =>
        await Response(
           GetQuoteEventHistory.Create(QuoteId.Of(quoteId))
       );

    /// <summary>
    /// Open a Quote for a customer
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.WritePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OpenCustomerQuote([FromBody] OpenQuoteRequest request) =>
        await Response(
           OpenQuote.Create(CustomerId.Of(request.CustomerId))
        );

    /// <summary>
    /// Add a Quote item
    /// </summary>
    /// <param name="quoteId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut, Route("{quoteId:guid}/items")]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.WritePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem([FromRoute] Guid quoteId, [FromBody] AddQuoteItemRequest request) =>
        await Response(
            AddQuoteItem.Create(
                QuoteId.Of(quoteId),
                ProductId.Of(request.ProductId),
                request.Quantity,
                Currency.OfCode(request.CurrencyCode)
            )
       );

    /// <summary>
    /// Delete a Quote item
    /// </summary>
    /// <param name="quoteId"></param>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpDelete, Route("{quoteId:guid}/items/{productId}")]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.DeletePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveItem([FromRoute] Guid quoteId, [FromRoute] Guid productId) =>
        await Response(
            RemoveQuoteItem.Create(QuoteId.Of(quoteId), ProductId.Of(productId))
        );

    /// <summary>
    /// Cancel a Quote
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpDelete, Route("{quoteId:guid}")]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.DeletePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel([FromRoute] Guid quoteId) =>
        await Response(
            CancelQuote.Create(QuoteId.Of(quoteId))
        );

    /// <summary>
    /// Confirms the quote
    /// </summary>
    /// <param name="quoteId"></param>
    /// <param name="currencyCode"></param>
    /// <returns></returns>
    [HttpPut, Route("{quoteId:guid}/confirm/{currencyCode}")]
    [Authorize(Roles = "Customer", Policy = PolicyBuilder.WritePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirm([FromRoute] Guid quoteId, [FromRoute] string currencyCode) =>    
        await Response(
            ConfirmQuote.Create(
            QuoteId.Of(quoteId), Currency.OfCode(currencyCode))
        );    
}