namespace EcommerceDDD.Orders.Tests;

public class OrdersControllerTests
{
    public OrdersControllerTests()
    {
        _ordersController = new OrdersController(
            _commandBus.Object,
            _queryBus.Object);
    }

    [Fact]
    public async Task ListCustomerOrders_WithQuoteId_ShouldReturnListOfOrderViewModel()
    {
        // Given
        var customerId = Guid.NewGuid();
        var quoteId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var expectedData = new List<OrderViewModel>
        {
            new OrderViewModel()
            {
                OrderId = orderId,
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                CurrencySymbol = Currency.USDollar.Symbol,
                QuoteId = quoteId,
                StatusCode = (int)OrderStatus.Placed,
                StatusText = OrderStatus.Placed.ToString(),
                TotalPrice = 10m,
                OrderLines = new List<OrderLineViewModel>()
                {
                    new OrderLineViewModel()
                    {
                        ProductId = Guid.NewGuid(),
                        ProductName = "Product X",
                        Quantity = 1,
                        UnitPrice = 10m
                    }
                }
            }
        };

        _queryBus
            .Setup(m => m.Send(It.IsAny<GetOrders>()))
            .ReturnsAsync(expectedData);

        // When
        var response = await _ordersController.ListCustomerOrders(customerId);

        // Then
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<IList<OrderViewModel>>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public async Task ListHistory_WithOrderId_ShouldReturnListOfOrderEventHistory()
    {
        // Given
        var customerId = Guid.NewGuid();
        var quoteId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var expectedData = new List<OrderEventHistory>
        {
            new OrderEventHistory(
                Guid.NewGuid(),
                orderId,
                typeof(OrderPlaced).Name,
                "event data"
            ),
            new OrderEventHistory(
                Guid.NewGuid(),
                orderId,
                typeof(OrderPaid).Name,
                "event data"
            )
        };

        _queryBus
            .Setup(m => m.Send(It.IsAny<GetOrderEventHistory>()))
            .ReturnsAsync(expectedData);

        // When
        var response = await _ordersController.ListHistory(customerId);

        // Then
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<IList<OrderEventHistory>>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public async Task PlaceOrderFromQuote_WithQuoteId_ShouldPlaceOrder()
    {
        // Given
        Guid quoteId = Guid.NewGuid();

        _commandBus
            .Setup(m => m.Send(It.IsAny<PlaceOrder>()));

        // When
        var response = await _ordersController.PlaceOrderFromQuote(quoteId);

        // Then
        response.Should().BeOfType<OkObjectResult>();
    }

    private Mock<ICommandBus> _commandBus = new();
    private Mock<IQueryBus> _queryBus = new();
    private OrdersController _ordersController;
}