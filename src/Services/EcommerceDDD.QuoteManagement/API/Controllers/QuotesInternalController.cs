namespace EcommerceDDD.QuoteManagement.API.Controllers;

[Authorize(Roles = Roles.M2MAccess)] // M2M only
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/internal/quotes")]
public class QuotesInternalController(
	ICommandBus commandBus,
	IQueryBus queryBus
) : CustomControllerBase(commandBus, queryBus)
{
	/// <summary>
	/// Confirms a quote
	/// </summary>
	/// <param name="quoteId"></param>
	/// <returns></returns>
	[HttpPut, Route("{quoteId:guid}/confirm")]
	[Authorize(Policy = Policies.CanWrite)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> Confirm([FromRoute] Guid quoteId,
		CancellationToken cancellationToken) =>
		await Response(
			ConfirmQuote.Create(QuoteId.Of(quoteId)),
			cancellationToken
		);

	/// <summary>
	/// Get a quote by quoteId
	/// </summary>
	/// <returns></returns>
	[HttpGet, Route("{quoteId:guid?}/details")]
	[Authorize(Policy = Policies.CanRead)]
	[ProducesResponseType(typeof(QuoteViewModel), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetQuoteDetails([FromRoute] Guid quoteId,
		CancellationToken cancellationToken) =>
		await Response(
			GetQuoteById.Create(QuoteId.Of(quoteId)),
			cancellationToken
		);
}
