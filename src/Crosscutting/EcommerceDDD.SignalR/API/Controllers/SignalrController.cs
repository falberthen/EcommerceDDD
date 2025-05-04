namespace EcommerceDDD.SignalR.API.Controllers;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/signalr")]
public class SignalrController(IOrderStatusUpdater orderStatusUpdater) : ControllerBase
{
	private IOrderStatusUpdater _orderStatusUpdater = orderStatusUpdater
		?? throw new ArgumentNullException(nameof(orderStatusUpdater));

	[HttpPost, Route("updateorderstatus")]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
	{
		await _orderStatusUpdater.UpdateOrderStatusAsync(
			request.CustomerId,
			request.OrderId,
			request.OrderStatusText,
			request.OrderStatusCode);

		return Ok();
	}
}