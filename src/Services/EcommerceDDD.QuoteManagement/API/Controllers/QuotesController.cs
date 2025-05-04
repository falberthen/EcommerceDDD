namespace EcommerceDDD.QuoteManagement.API.Controllers;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/quotes")]
public class QuotesController(
	ICommandBus commandBus,
	IQueryBus queryBus
) : CustomControllerBase(commandBus, queryBus)
{
	/// <summary>
	/// Get the current customer's quote
	/// </summary>
	/// <returns></returns>
	[HttpGet]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<QuoteViewModel?>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetOpenQuote(CancellationToken cancellationToken) =>
		await Response(
		   GetCustomerOpenQuote.Create(), cancellationToken
	   );

	/// <summary>
	/// Creates a quote for a customer
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpPost]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanWrite)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> OpenQuoteForCustomer([FromBody] OpenQuoteRequest request,
		CancellationToken cancellationToken) =>
		await Response(
		   OpenQuote.Create(Currency.OfCode(request.CurrencyCode)),
		   cancellationToken
		);

	/// <summary>
	/// Get quote event history
	/// </summary>
	/// <param name="quoteId"></param>
	/// <returns></returns>
	[HttpGet, Route("{quoteId:guid}/history")]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<IEventHistory>>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ListHistory([FromRoute] Guid quoteId,
		CancellationToken cancellationToken) =>
		await Response(
		   GetQuoteEventHistory.Create(QuoteId.Of(quoteId)),
		   cancellationToken
	   );

	/// <summary>
	/// Add a quote item
	/// </summary>
	/// <param name="quoteId"></param>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpPut, Route("{quoteId:guid}/items")]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanWrite)]
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
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanDelete)]
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
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanDelete)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Cancel([FromRoute] Guid quoteId,
		CancellationToken cancellationToken) =>
		await Response(
			CancelQuote.Create(QuoteId.Of(quoteId)),
			cancellationToken
		);

	/// <summary>
	/// Confirms a quote | M2M only
	/// </summary>
	/// <param name="quoteId"></param>
	/// <returns></returns>
	[HttpPut, Route("{quoteId:guid}/confirm")]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.M2MAccess, Policy = Policies.CanWrite)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Confirm([FromRoute] Guid quoteId,
		CancellationToken cancellationToken) =>
		await Response(
			ConfirmQuote.Create(
				QuoteId.Of(quoteId)),
			cancellationToken
		);

	/// <summary>
	/// Get a quote by quoteId | M2M only
	/// </summary>
	/// <returns></returns>
	[HttpGet, Route("{quoteId:guid?}/details")]
	[MapToApiVersion(ApiVersions.V2)]
	[Authorize(Roles = Roles.M2MAccess, Policy = Policies.CanRead)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<QuoteViewModel?>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetQuoteDetails([FromRoute] Guid quoteId,
		CancellationToken cancellationToken) =>
		await Response(
		   GetQuoteById.Create(QuoteId.Of(quoteId)),
		   cancellationToken
	   );
}