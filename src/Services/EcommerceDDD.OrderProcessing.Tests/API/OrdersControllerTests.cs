namespace EcommerceDDD.OrderProcessing.Tests;

public class OrdersControllerTests
{
    public OrdersControllerTests()
    {
        _ordersController = new OrdersController(
            _commandBus, _queryBus);
    }

    [Fact]
    public async Task ListCustomerOrders_ShouldReturnListOfOrderViewModel()
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

        _queryBus.SendAsync(Arg.Any<GetOrders>(), Arg.Any<CancellationToken>())
            .Returns(expectedData);

        // When
        var response = await _ordersController.ListCustomerOrders(CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		var apiResponse = Assert.IsType<ApiResponse<IList<OrderViewModel>>>(okResult.Value);
		Assert.IsAssignableFrom<IList<OrderViewModel>>(apiResponse.Data);
	}

    [Fact]
    public async Task ListHistory_WithOrderId_ShouldReturnListOfOrderEventHistory()
    {
        // Given
        var customerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var expectedData = new List<OrderEventHistory>
        {
            new OrderEventHistory(
                Guid.NewGuid(),
                orderId,
                typeof(OrderPlaced).Name,
                "event data",
                DateTime.UtcNow
            ),
            new OrderEventHistory(
                Guid.NewGuid(),
                orderId,
                typeof(OrderPaid).Name,
                "event data",
                DateTime.UtcNow
            )
        };

        _queryBus.SendAsync(Arg.Any<GetOrderEventHistory>(), CancellationToken.None)
            .Returns(expectedData);

        // When
        var response = await _ordersController.ListHistory(customerId, CancellationToken.None);

		// Then		
		var okResult = Assert.IsType<OkObjectResult>(response);
		var apiResponse = Assert.IsType<ApiResponse<IList<OrderEventHistory>>>(okResult.Value);
		Assert.IsAssignableFrom<IList<OrderEventHistory>>(apiResponse.Data);
	}

    [Fact]
    public async Task PlaceOrderFromQuote_WithQuoteId_ShouldPlaceOrder()
    {
        // Given
        Guid customerId = Guid.NewGuid();
        Guid quoteId = Guid.NewGuid();

        await _commandBus.SendAsync(Arg.Any<PlaceOrder>(), CancellationToken.None);

        // When
        var response = await _ordersController
            .PlaceOrderFromQuote(quoteId, CancellationToken.None);

        // Then        
		Assert.IsType<OkObjectResult>(response);
    }

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IQueryBus _queryBus = Substitute.For<IQueryBus>();
    private OrdersController _ordersController;
}