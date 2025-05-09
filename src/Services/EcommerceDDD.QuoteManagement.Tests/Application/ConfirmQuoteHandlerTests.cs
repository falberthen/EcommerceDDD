namespace EcommerceDDD.QuoteManagement.Tests.Application;

public class ConfirmQuoteHandlerTests
{
	[Fact]
	public async Task ConfirmQuote_WithCommand_ShouldConfirmQuote()
	{
		// Given
		var customerId = CustomerId.Of(Guid.NewGuid());
		var productId = ProductId.Of(Guid.NewGuid());
		const int productQuantity = 1;
		var currency = Currency.OfCode(Currency.USDollar.Code);

		var quote = Quote.OpenQuoteForCustomer(customerId, currency);
		quote.AddItem(new QuoteItemData(quote.Id, productId, "Product",
			Money.Of(10, currency.Code), productQuantity));

		var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
		await quoteWriteRepository.AppendEventsAsync(quote);

		var confirmQuote = ConfirmQuote.Create(quote.Id);
		var confirmQuoteHandler = new ConfirmQuoteHandler(quoteWriteRepository);

		// When
		await confirmQuoteHandler.HandleAsync(confirmQuote, CancellationToken.None);

		// Then
		var storedQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
		Assert.Equal(QuoteStatus.Confirmed, storedQuote.Status);
	}

	[Fact]
	public async Task ConfirmQuote_WithoutItems_ShouldThrowException()
	{
		// Given
		var customerId = CustomerId.Of(Guid.NewGuid());
		var currency = Currency.OfCode(Currency.USDollar.Code);
		var quote = Quote.OpenQuoteForCustomer(customerId, currency);
		var quoteWriteRepository = new DummyEventStoreRepository<Quote>();

		await quoteWriteRepository.AppendEventsAsync(quote);
		var confirmQuote = ConfirmQuote.Create(quote.Id);
		var confirmQuoteHandler = new ConfirmQuoteHandler(quoteWriteRepository);

		// When & Then
		await Assert.ThrowsAsync<BusinessRuleException>(() =>
			confirmQuoteHandler.HandleAsync(confirmQuote, CancellationToken.None));
	}
}