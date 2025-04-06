namespace EcommerceDDD.OrderProcessing.API.Controllers;

[Route("api/orders")]
[ApiController]
public class OrdersController(
    ICommandBus commandBus,
    IQueryBus queryBus
) : CustomControllerBase(commandBus, queryBus)
{
    /// <summary>
    /// Get customer's orders
    /// </summary>
    /// <returns></returns>
    [HttpGet]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<OrderViewModel>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListCustomerOrders(CancellationToken cancellationToken) => 
        await Response(
			GetOrders.Create(), cancellationToken
		);

    /// <summary>
    /// Get order event history
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet, Route("{orderId:guid}/history")]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanRead)]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<IEventHistory>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid orderId, CancellationToken cancellationToken) =>  
        await Response(
            GetOrderEventHistory.Create(OrderId.Of(orderId)),
            cancellationToken
        );

    /// <summary>
    /// Places an order from a quote
    /// </summary>
    /// <param name="quoteId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost, Route("quote/{quoteId:guid}")]
	[Authorize(Roles = Roles.Customer, Policy = Policies.CanWrite)]
	[ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrderFromQuote([FromRoute] Guid quoteId, CancellationToken cancellationToken) =>
        await Response(
            PlaceOrder.Create(QuoteId.Of(quoteId)), cancellationToken
		);
}