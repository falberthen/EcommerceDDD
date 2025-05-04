namespace EcommerceDDD.OrderProcessing.Tests.Application;

public class CancelOrderHandlerTests
{
	[Fact]
	public async Task CancelOrder_WithCommand_ShouldCancelOrder()
	{
		// Given
		var quoteId = QuoteId.Of(Guid.NewGuid());
		var customerId = CustomerId.Of(Guid.NewGuid());

		var orderData = new OrderData(customerId, quoteId);
		var order = Order.Place(orderData);

		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		await orderWriteRepository
			.AppendEventsAsync(order);

		// mocked kiota request
		var apiClient = new ApiGatewayClient(_requestAdapter);
		_requestAdapter.SendPrimitiveAsync<string>(
			Arg.Any<RequestInformation>(),
			Arg.Any<Dictionary<string, ParsableFactory<IParsable>>>(),
			Arg.Any<CancellationToken>())
			.Returns("Success");

		var cancelOrder = CancelOrder.Create(order.Id, OrderCancellationReason.CanceledByCustomer);
		var cancelOrderHandler = new CancelOrderHandler(apiClient, orderWriteRepository);

		// When
		await cancelOrderHandler.HandleAsync(cancelOrder, CancellationToken.None);

		// Then
		var canceledOrder = orderWriteRepository.AggregateStream.First().Aggregate;
		Assert.NotNull(canceledOrder);
		Assert.Equal(canceledOrder.CustomerId, customerId);
		Assert.Equal(canceledOrder.QuoteId, quoteId);
		Assert.Equal(OrderStatus.Canceled, canceledOrder.Status);
	}

	private IRequestAdapter _requestAdapter = Substitute.For<IRequestAdapter>();
}