using EcommerceDDD.WebApi.Controllers.Base;
using EcommerceDDD.Application.Quotes.GetQuoteDetails;
using EcommerceDDD.Application.Quotes.SaveQuote;
using EcommerceDDD.Application.Quotes.GetCurrentQuote;
using EcommerceDDD.Application.Quotes.ChangeQuote;

namespace EcommerceDDD.WebApi.Controllers;

[Authorize]
[Route("api/quotes")]
[ApiController]
public class QuotesController : BaseController
{
    public QuotesController(
        IMediator mediator)
        : base(mediator)
    {
    }

    /// <summary>
    /// Add a Quote 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "CanSave")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateQuote([FromBody] CreateQuoteRequest request)
    {
        var command = new CreateQuoteCommand(request.CustomerId, request.Product);
        return await Response(command);
    }

    /// <summary>
    /// Change a Quote 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Policy = "CanSave")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeQuote([FromBody] ChangeQuoteRequest request)
    {
        var command = new ChangeQuoteCommand(request.QuoteId, request.Product);
        return await Response(command);
    }

    /// <summary>
    /// Returns the current customer's quote
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}/quote")]
    [Authorize(Policy = "CanRead")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCurrentQuote([FromRoute] Guid customerId)
    {
        var query = new GetCurrentQuoteQuery(customerId);
        return await Response(query);
    }

    /// <summary>
    /// Returns the quote details
    /// </summary>
    /// <param name="quoteId"></param>
    /// <param name="currency"></param>
    /// <returns></returns>
    [HttpGet, Route("{quoteId:guid}/details/{currency}")]
    [Authorize(Policy = "CanRead")]
    [ProducesResponseType(typeof(QuoteDetailsViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetQuoteDetails([FromRoute] Guid quoteId, [FromRoute] string currency)
    {
        var query = new GetQuoteDetailsQuery(quoteId, currency);
        return await Response(query);
    }
}