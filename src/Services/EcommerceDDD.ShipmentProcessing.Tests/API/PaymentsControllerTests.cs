namespace EcommerceDDD.ShipmentProcessing.Tests;

public class ShipmentsControllerTests
{
    public ShipmentsControllerTests()
    {
        _shipmentsController = new ShipmentsController(
            _commandBus, _queryBus);
    }

    [Fact]
    public async Task RequestOrderShipment_WithShipOrderRequest_ShouldRequestShipment()
    {
        // Given
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        await _commandBus.SendAsync(Arg.Any<RequestShipment>(), CancellationToken.None);

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
        response.Should().BeOfType<OkObjectResult>();
    }

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IQueryBus _queryBus = Substitute.For<IQueryBus>();
    private ShipmentsController _shipmentsController;
}