namespace EcommerceDDD.ApiGateway.SignalR;

[Authorize]
[Route("api/signalr")]
[ApiController]
public class SignalrController : ControllerBase
{
    private IOrderStatusUpdater _orderStatusUpdater;

    public SignalrController(IOrderStatusUpdater orderStatusUpdater)
    {
        _orderStatusUpdater = orderStatusUpdater;
    }

    [HttpPost, Route("updateorderstatus")]
    public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
    {
        await _orderStatusUpdater.UpdateOrderStatus(request.CustomerId,
            request.OrderId,
            request.OrderStatusText,
            request.OrderStatusCode);

        return Ok();
    }

    public record UpdateOrderStatusRequest(Guid CustomerId, Guid OrderId, string OrderStatusText, int OrderStatusCode);
}
