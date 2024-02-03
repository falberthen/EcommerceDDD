namespace EcommerceDDD.QuoteManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/quotes")]
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
    /// <param name="currencyCode"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}/quote/{quoteId:guid?}")]
    [Authorize(Policy = Policies.CanRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<QuoteViewModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetQuote([FromRoute] Guid customerId, [FromRoute] Guid? quoteId,
        CancellationToken cancellationToken) =>
        await Response(
           GetCustomerQuote.Create(
               CustomerId.Of(customerId), 
               quoteId == null ? null : QuoteId.Of(quoteId.Value!)),
           cancellationToken
       );    

    /// <summary>
    /// Get quote event history
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpGet, Route("{quoteId:guid}/history")]
    [Authorize(Policy = Policies.CanRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<QuoteEventHistory>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid quoteId,
        CancellationToken cancellationToken) =>
        await Response(
           GetQuoteEventHistory.Create(QuoteId.Of(quoteId)),
           cancellationToken
       );

    /// <summary>
    /// Creates a quote for a customer
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OpenCustomerQuote([FromBody] OpenQuoteRequest request,
        CancellationToken cancellationToken) =>
        await Response(
           OpenQuote.Create(
               CustomerId.Of(request.CustomerId),
               Currency.OfCode(request.CurrencyCode)),
           cancellationToken
        );

    /// <summary>
    /// Add a quote item
    /// </summary>
    /// <param name="quoteId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut, Route("{quoteId:guid}/items")]
    [Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem([FromRoute] Guid quoteId, 
        [FromBody] AddQuoteItemRequest request, CancellationToken cancellationToken) =>
        await Response(
            AddQuoteItem.Create(
                QuoteId.Of(quoteId),
                ProductId.Of(request.ProductId),
                request.Quantity              
            ), cancellationToken
       );

    /// <summary>
    /// Delete a quote item
    /// </summary>
    /// <param name="quoteId"></param>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpDelete, Route("{quoteId:guid}/items/{productId:guid}")]
    [Authorize(Policy = Policies.CanDelete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveItem([FromRoute] Guid quoteId, [FromRoute] Guid productId,
        CancellationToken cancellationToken) =>
        await Response(
            RemoveQuoteItem.Create(QuoteId.Of(quoteId), ProductId.Of(productId)),
            cancellationToken
        );

    /// <summary>
    /// Cancel a quote
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpDelete, Route("{quoteId:guid}")]
    [Authorize(Policy = Policies.CanDelete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel([FromRoute] Guid quoteId,
        CancellationToken cancellationToken) =>
        await Response(
            CancelQuote.Create(QuoteId.Of(quoteId)),
            cancellationToken
        );

    /// <summary>
    /// Confirms a quote
    /// </summary>
    /// <param name="quoteId"></param>
    /// <returns></returns>
    [HttpPut, Route("{quoteId:guid}/confirm")]
    [Authorize(Policy = Policies.M2MAccess)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirm([FromRoute] Guid quoteId,
        CancellationToken cancellationToken) =>
        await Response(
            ConfirmQuote.Create(
                QuoteId.Of(quoteId)),
            cancellationToken
        );
}