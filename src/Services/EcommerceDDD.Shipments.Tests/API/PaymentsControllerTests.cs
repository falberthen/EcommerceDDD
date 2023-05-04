namespace EcommerceDDD.Shipments.Tests;

public class ShipmentsControllerTests
{
    public ShipmentsControllerTests()
    {
        _shipmentsController = new ShipmentsController(
            _commandBus.Object,
            _queryBus.Object);
    }

    [Fact]
    public async Task RequestOrderShipment_WithShipOrderRequest_ShouldRequestShipment()
    {
        // Given
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        _commandBus
            .Setup(m => m.Send(It.IsAny<RequestShipment>()));

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
        var response = await _shipmentsController.RequestOrderShipment(request);

        // Then
        response.Should().BeOfType<OkObjectResult>();
    }

    private Mock<ICommandBus> _commandBus = new();
    private Mock<IQueryBus> _queryBus = new();
    private ShipmentsController _shipmentsController;
}