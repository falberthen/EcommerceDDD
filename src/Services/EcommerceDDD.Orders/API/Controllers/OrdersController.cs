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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListDetails([FromRoute] Guid customerId)
    {
        var query = GetOrders.Create(CustomerId.Of(customerId));
        return await Response(query);
    }

    /// <summary>
    /// Get order event history
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet, Route("{orderId}/history")]
    [Authorize(Policy = PolicyBuilder.ReadPolicy)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListHistory([FromRoute] Guid orderId)
    {
        var query = GetOrderEventHistory.Create(OrderId.Of(orderId));
        return await Response(query);
    }

    /// <summary>
    /// Places an order from a quote
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost, Route("{quoteId}")]
    [Authorize(Policy = PolicyBuilder.WritePolicy)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrderFromQuote([FromRoute] Guid quoteId)
    {
        var command = PlaceOrder.Create(QuoteId.Of(quoteId));
        return await Response(command);
    }
}