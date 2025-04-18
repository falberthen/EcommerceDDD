namespace EcommerceDDD.QuoteManagement.Tests.Domain;

public class QuoteEventsTests
{
    [Fact]
    public void OpenQuote_WithCustomerIdAndCurrency_ShouldApplyQuoteOpenEvent()
    {
        // Given
        var currency = Currency.OfCode(Currency.USDollar.Code);

        // When
        var quote = Quote.OpenQuoteForCustomer(_customerId, currency);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteOpen;
        Assert.NotNull(@event);
		Assert.IsType<QuoteOpen>(@event);
	}

    [Fact]
    public void Cancel_WithQuote_ShouldApplyQuoteCanceledEvent()
    {
        // Given
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var quote = Quote.OpenQuoteForCustomer(_customerId, currency); 
        
        // When
        quote.Cancel();

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteCanceled;
        Assert.NotNull(@event);
		Assert.IsType<QuoteCanceled>(@event);
	}

    [Fact]
    public void AddingItem_WithQuoteItemData_ShouldApplyQuoteItemAddedEvent()
    {
        // Given
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var quoteItem = new QuoteItemData(_quoteId, _productId, "Product",
            Money.Of(10, currency.Code), 1);

        var quote = Quote.OpenQuoteForCustomer(_customerId, currency);

        // When        
        quote.AddItem(quoteItem);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteItemAdded;
        Assert.NotNull(@event);
		Assert.IsType<QuoteItemAdded>(@event);
    }

    [Fact]
    public void RemovingItem_WithProductId_ShouldApplyQuoteItemRemovedEvent()
    {
        // Given
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var customerId = CustomerId.Of(Guid.NewGuid());
        var quoteItem = new QuoteItemData(_quoteId, _productId, "Product",
            Money.Of(10, currency.Code), 1);

        var quote = Quote.OpenQuoteForCustomer(customerId, currency);
        quote.AddItem(quoteItem);

        // When        
        quote.RemoveItem(_productId);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteItemRemoved;
        Assert.NotNull(@event);        
		Assert.IsType<QuoteItemRemoved>(@event);
	}

    [Fact]
    public void AddItem_ForExistingItem_ShouldApplyQuoteItemQuantityChangedEvent()
    {
        // Given
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var quoteItem = new QuoteItemData(_quoteId, _productId, "Product",
            Money.Of(10, currency.Code), 1);

        var quote = Quote.OpenQuoteForCustomer(_customerId, currency);
        quote.AddItem(quoteItem);

        // When        
        quoteItem = quoteItem with { Quantity = 2 };
        quote.AddItem(quoteItem);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteItemQuantityChanged;
        Assert.NotNull(@event);
		Assert.IsType<QuoteItemQuantityChanged>(@event);
	}

    [Fact]
    public void Confirm_WithQuote_ShouldApplyQuoteConfirmedEvent()
    {
        // Given
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var quoteItem = new QuoteItemData(_quoteId, _productId, "Product",
            Money.Of(10, currency.Code), 1);
        var quote = Quote.OpenQuoteForCustomer(_customerId, currency);
        quote.AddItem(quoteItem);

        // When        
        quote.Confirm();

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteConfirmed;
        Assert.NotNull(@event);
		Assert.IsType<QuoteConfirmed>(@event);
    }

    private QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());
    private ProductId _productId = ProductId.Of(Guid.NewGuid());
    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
}