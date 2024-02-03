namespace EcommerceDDD.OrderProcessing.API.Controllers;

[Route("api/orders")]
[ApiController]
public class OrdersController : CustomControllerBase
{
    public OrdersController(
        ICommandBus commandBus,
        IQueryBus queryBus)
        : base(commandBus, queryBus) { }

    /// <summary>
    /// Get customer's orders
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet, Route("{customerId:guid}")]
    [Authorize(Policy = Policies.CanRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<OrderViewModel>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListCustomerOrders([FromRoute] Guid customerId, 
        CancellationToken cancellationToken) => 
         await Response(
             GetOrders.Create(CustomerId.Of(customerId)),
             cancellationToken
        );

    /// <summary>
    /// Get order event history
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet, Route("{orderId:guid}/history")]
    [Authorize(Policy = Policies.CanRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<OrderEventHistory>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid orderId
        , CancellationToken cancellationToken) =>  
        await Response(
            GetOrderEventHistory.Create(OrderId.Of(orderId)),
            cancellationToken
        );

    /// <summary>
    /// Places an order from a quote
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="quoteId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost, Route("{customerId:guid}/{quoteId:guid}")]
    [Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrderFromQuote([FromRoute] Guid customerId, 
        [FromRoute] Guid quoteId, CancellationToken cancellationToken) =>
        await Response(
            PlaceOrder.Create(CustomerId.Of(customerId),
                QuoteId.Of(quoteId)),
            cancellationToken
        );
}