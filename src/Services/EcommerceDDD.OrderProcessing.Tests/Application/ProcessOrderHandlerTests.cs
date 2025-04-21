using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.OrderProcessing.Tests.Application;

public class ProcessOrderHandlerTests
{
	[Fact]
	public async Task PlaceOrder_WithCommand_ShouldPlaceOrder()
	{
		// Given
		var productId = ProductId.Of(Guid.NewGuid());
		var productName = "Product XYZ";
		var productPrice = Money.Of(10, Currency.USDollar.Code);
		var customerId = CustomerId.Of(Guid.NewGuid());
		var currency = Currency.OfCode(Currency.USDollar.Code);
		var quoteId = QuoteId.Of(Guid.NewGuid());

		var quoteItems = new List<ProductItemData>() {
			new ProductItemData() {
				ProductId = productId,
				ProductName = productName,
				Quantity = 1,
				UnitPrice = productPrice
			}
		};

		var orderData = new OrderData(customerId, quoteId, currency, quoteItems);
		var order = Order.Place(orderData);

		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		var adapter = Substitute.For<IRequestAdapter>();
		var apiClient = new ApiGatewayClient(adapter);

		// return mocked view model
		var viewModelResponse = new QuoteViewModel()
		{
			QuoteId = quoteId.Value,
			CustomerId = customerId.Value,
			CurrencySymbol = currency.Symbol,
			CurrencyCode = currency.Code,
			Items = new List<QuoteItemViewModel>()
			{
				new QuoteItemViewModel()
				{
					ProductId = productId.Value,
					ProductName = "Product",
					Quantity = 10,
					UnitPrice = 200
				}
			}
		};

		var quoteApiResponse = new QuoteViewModelApiResponse()
		{
			Data = viewModelResponse,
			Success = true
		};

		// mocked kiota request
		adapter.SendAsync(
			Arg.Is<RequestInformation>(req => req.PathParameters.Values.Contains(quoteId.Value)),
			Arg.Any<ParsableFactory<QuoteViewModelApiResponse>>(),
			Arg.Any<Dictionary<string, ParsableFactory<IParsable>>>(),
			Arg.Any<CancellationToken>())
		.Returns(quoteApiResponse);

		await orderWriteRepository
			.AppendEventsAsync(order);

		var processOrder = ProcessOrder.Create(customerId, order.Id, quoteId);
		var processOrderHandler = new ProcessOrderHandler(apiClient, orderWriteRepository, _eventPublisher);

		// When
		await processOrderHandler.HandleAsync(processOrder, CancellationToken.None);

		// Then
		var placedOrder = orderWriteRepository.AggregateStream.First().Aggregate;
		Assert.NotNull(placedOrder);
		Assert.Equal(placedOrder.CustomerId, customerId);
		Assert.Equal(placedOrder.QuoteId, quoteId);
		Assert.Equal(OrderStatus.Processed, placedOrder.Status);
	}

	private IEventBus _eventPublisher = Substitute.For<IEventBus>();

}