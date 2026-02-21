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
            .Returns(Result.Ok<IReadOnlyList<OrderViewModel>>(expectedData));

        // When
        var response = await _ordersController.ListCustomerOrders(CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		Assert.IsAssignableFrom<IReadOnlyList<OrderViewModel>>(okResult.Value);
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
            .Returns(Result.Ok<IReadOnlyList<OrderEventHistory>>(expectedData));

        // When
        var response = await _ordersController.ListHistory(customerId, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		Assert.IsAssignableFrom<IReadOnlyList<OrderEventHistory>>(okResult.Value);
	}

    [Fact]
    public async Task PlaceOrderFromQuote_WithQuoteId_ShouldPlaceOrder()
    {
        // Given
        Guid customerId = Guid.NewGuid();
        Guid quoteId = Guid.NewGuid();

        _commandBus.SendAsync(Arg.Any<PlaceOrder>(), CancellationToken.None)
            .Returns(Result.Ok());

        // When
        var response = await _ordersController
            .PlaceOrderFromQuote(quoteId, CancellationToken.None);

        // Then
		Assert.IsType<OkResult>(response);
    }

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IQueryBus _queryBus = Substitute.For<IQueryBus>();
    private OrdersController _ordersController;
}
