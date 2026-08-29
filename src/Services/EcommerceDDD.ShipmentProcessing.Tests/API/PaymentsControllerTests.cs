namespace EcommerceDDD.ShipmentProcessing.Tests;

public class ShipmentsControllerTests
{
    public ShipmentsControllerTests()
    {
        _shipmentsController = new ShipmentsController(_bus);
    }

    [Fact]
    public async Task RequestOrderShipment_WithShipOrderRequest_ShouldRequestShipment()
    {
        // Given
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        _bus.InvokeAsync<Result>(Arg.Any<RequestShipment>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
			.Returns(Task.FromResult(Result.Ok()));

        var request = new ShipOrderRequest()
        {
            OrderId = orderId,
            ProductItems = new List<ProductItemRequest>
            {
                new ProductItemRequest(
                    productId, 
                    "Product X", 
                    10m, 
                    1
                )
            }
        };

        // When
        var response = await _shipmentsController.RequestOrderShipment(request, 
            CancellationToken.None);

        // Then
		Assert.IsType<OkResult>(response);
	}

    private IMessageBus _bus = Substitute.For<IMessageBus>();
    private ShipmentsController _shipmentsController;
}