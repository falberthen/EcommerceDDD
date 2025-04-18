namespace EcommerceDDD.OrderProcessing.Tests.Domain;

public class OrderTests
{
	[Fact]
	public void Place_FromQuote_ShouldPlaceOrder()
	{
		var orderData = new OrderData(_customerId, _quoteId, currency);

		// When
		var order = Order.Place(orderData);

		// Then
		Assert.NotNull(order);
		Assert.Equal(order.QuoteId.Value, _quoteId.Value);
		Assert.Equal(order.CustomerId.Value, _customerId.Value);
		Assert.Equal(OrderStatus.Placed, order.Status);
	}

	[Fact]
	public void Process_WithQuote_ShouldProcessOrder()
	{
		// Given
		var orderData = new OrderData(_customerId, _quoteId, currency);
		var order = Order.Place(orderData);

		var productId = ProductId.Of(Guid.NewGuid());
		var productPrice = Money.Of(15, Currency.USDollar.Code);
		const string productName = "Product XYZ";

		var quoteItems = new List<ProductItemData>() {
			new ProductItemData() {
				ProductId = productId,
				ProductName = productName,
				Quantity = 1,
				UnitPrice = productPrice
			}
		};

		// Adding quote items
		orderData = new OrderData(_customerId, _quoteId, currency, quoteItems);

		// When
		order.Process(orderData);

		// Then
		Assert.NotNull(order);
		Assert.Equal(order.QuoteId.Value, _quoteId.Value);
		Assert.Equal(order.CustomerId.Value, _customerId.Value);
		Assert.Equal(OrderStatus.Processed, order.Status);
	}

	[Fact]
	public void Process_WithEmptyItems_ShouldThrowException()
	{
		// Given
		var orderData = new OrderData(_customerId, _quoteId, currency);
		var order = Order.Place(orderData);

		// When & Then
		Assert.Throws<BusinessRuleException>(() => 
			order.Process(orderData));
	}

	private QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());
	private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
	private Currency currency = Currency.OfCode(Currency.USDollar.Code);
}