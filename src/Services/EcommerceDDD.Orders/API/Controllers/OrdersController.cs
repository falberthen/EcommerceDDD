namespace EcommerceDDD.Orders.API.Controllers;

[Authorize(Roles = "Customer")]
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
    [Authorize(Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<OrderViewModel>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListCustomerOrders([FromRoute] Guid customerId) => 
         await Response(
             GetOrders.Create(CustomerId.Of(customerId))
        );
    

    /// <summary>
    /// Get order event history
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet, Route("{orderId}/history")]
    [Authorize(Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IList<OrderEventHistory>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid orderId) =>  
        await Response(
            GetOrderEventHistory.Create(OrderId.Of(orderId))
        );

    /// <summary>
    /// Places an order from a quote
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost, Route("{quoteId}")]
    [Authorize(Policy = PolicyBuilder.WritePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrderFromQuote([FromRoute] Guid quoteId) =>
        await Response(
            PlaceOrder.Create(QuoteId.Of(quoteId))
        );
}