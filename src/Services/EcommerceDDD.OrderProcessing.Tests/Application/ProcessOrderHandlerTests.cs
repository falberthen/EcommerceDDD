using EcommerceDDD.OrderProcessing.Application.Payments.ProcessingPayment.IntegrationEvents;

namespace EcommerceDDD.OrderProcessing.Tests.Application;

public class ProcessOrderHandlerTests
{
	[Fact]
	public async Task ProcessOrder_WithStockAvailable_ShouldProcessOrder()
	{
		// Given
		var productId = ProductId.Of(Guid.NewGuid());
		var customerId = CustomerId.Of(Guid.NewGuid());
		var currency = Currency.OfCode(Currency.USDollar.Code);
		var quoteId = QuoteId.Of(Guid.NewGuid());

		var orderData = BuildOrderData(customerId, quoteId, currency, productId);
		var order = Order.Place(orderData);

		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		await orderWriteRepository.AppendEventsAndCommitAsync(order);

		var quoteService = Substitute.For<IQuoteService>();
		quoteService.GetQuoteDetailsAsync(quoteId.Value, Arg.Any<CancellationToken>())
			.Returns(BuildQuote(quoteId, customerId, currency, productId));

		_productInventoryHandler
			.CheckProductsInStockAsync(Arg.Any<IReadOnlyList<ProductItemData>>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult(true));

		var processOrder = ProcessOrder.Create(customerId, order.Id, quoteId);
		var processOrderHandler = new ProcessOrderHandler(
			quoteService, _productInventoryHandler, orderWriteRepository, _messageBus);

		// When
		await processOrderHandler.HandleAsync(processOrder, CancellationToken.None);

		// Then
		var processedOrder = orderWriteRepository.AggregateStream.First().Aggregate;
		Assert.NotNull(processedOrder);
		Assert.Equal(OrderStatus.Processed, processedOrder.Status);
		await _productInventoryHandler.Received(1)
			.DecreaseQuantityInStockAsync(Arg.Any<IReadOnlyList<ProductItemData>>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ProcessOrder_WhenOutOfStock_ShouldNotProcessAndSignalOutOfStock()
	{
		// Given
		var productId = ProductId.Of(Guid.NewGuid());
		var customerId = CustomerId.Of(Guid.NewGuid());
		var currency = Currency.OfCode(Currency.USDollar.Code);
		var quoteId = QuoteId.Of(Guid.NewGuid());

		var orderData = BuildOrderData(customerId, quoteId, currency, productId);
		var order = Order.Place(orderData);

		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		await orderWriteRepository.AppendEventsAndCommitAsync(order);

		var quoteService = Substitute.For<IQuoteService>();
		quoteService.GetQuoteDetailsAsync(quoteId.Value, Arg.Any<CancellationToken>())
			.Returns(BuildQuote(quoteId, customerId, currency, productId));

		_productInventoryHandler
			.CheckProductsInStockAsync(Arg.Any<IReadOnlyList<ProductItemData>>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult(false));

		var processOrder = ProcessOrder.Create(customerId, order.Id, quoteId);
		var processOrderHandler = new ProcessOrderHandler(
			quoteService, _productInventoryHandler, orderWriteRepository, _messageBus);

		// When
		await processOrderHandler.HandleAsync(processOrder, CancellationToken.None);

		// Then
		var stillPlaced = orderWriteRepository.AggregateStream.First().Aggregate;
		Assert.Equal(OrderStatus.Placed, stillPlaced.Status);
		await _productInventoryHandler.DidNotReceive()
			.DecreaseQuantityInStockAsync(Arg.Any<IReadOnlyList<ProductItemData>>(), Arg.Any<CancellationToken>());
		await _messageBus.Received(1).PublishAsync(Arg.Any<ProductWasOutOfStock>());
	}

	private static OrderData BuildOrderData(CustomerId customerId, QuoteId quoteId, Currency currency, ProductId productId) =>
		new OrderData(customerId, quoteId, currency, new List<ProductItemData>() {
			new ProductItemData() {
				ProductId = productId,
				ProductName = "Product XYZ",
				Quantity = 1,
				UnitPrice = Money.Of(10, currency.Code)
			}
		});

	private static QuoteViewModel BuildQuote(QuoteId quoteId, CustomerId customerId, Currency currency, ProductId productId) =>
		new QuoteViewModel()
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
					ProductName = "Product XYZ",
					Quantity = 10,
					UnitPrice = 200
				}
			}
		};

	private IMessageBus _messageBus = Substitute.For<IMessageBus>();
	private IProductInventoryHandler _productInventoryHandler = Substitute.For<IProductInventoryHandler>();
}
