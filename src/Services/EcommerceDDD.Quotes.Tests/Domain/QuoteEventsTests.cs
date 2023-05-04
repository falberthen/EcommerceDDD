namespace EcommerceDDD.Quotes.Tests.Domain;

public class QuoteEventsTests
{
    [Fact]
    public void CreatingQuote_WithCustomerId_ReturnsQuoteOpenEvent()
    {
        // When
        var quote = Quote.Create(_customerId);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteOpen;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteOpen>();
    }

    [Fact]
    public void CancellingQuote_WithCustomerId_ReturnsQuoteCanceledEvent()
    {
        // When
        var quote = Quote.Create(_customerId);
        quote.Cancel();

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteCanceled;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteCanceled>();
    }

    [Fact]
    public void AddingItemToQuote_WithQuoteItemData_ReturnsQuoteItemAddedEvent()
    {
        // Given
        var quoteItem = new QuoteItemData(_quoteId, _productId, 1);

        // When
        var quote = Quote.Create(_customerId);
        quote.AddItem(quoteItem);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteItemAdded;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteItemAdded>();
    }

    [Fact]
    public void RemovingItemFromQuote_WithQuoteItemData_ReturnsQuoteItemRemovedEvent()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        var quoteItemData = new QuoteItemData(QuoteId.Of(Guid.NewGuid()), _productId, 1);

        // When
        var quote = Quote.Create(customerId);
        quote.AddItem(quoteItemData);
        quote.RemoveItem(_productId);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteItemRemoved;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteItemRemoved>();
    }

    [Fact]
    public void ChangingQuoteItemQuantity_WithQuoteItemData_ReturnsQuoteItemQuantityChangedEvent()
    {
        // Given
        var quoteItemData = new QuoteItemData(_quoteId, _productId, 1);

        // When
        var quote = Quote.Create(_customerId);
        quote.AddItem(quoteItemData);
        quoteItemData = quoteItemData with { Quantity = 2 };
        quote.AddItem(quoteItemData);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteItemQuantityChanged;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteItemQuantityChanged>();
    }

    [Fact]
    public void ConfirmingQuote_WithQuoteItemData_ReturnsQuoteConfirmedEvent()
    {
        // Given
        var quoteItemData = new QuoteItemData(_quoteId, _productId, 1);
        var currency = Currency.OfCode(Currency.CanadianDollar.Code);

        // When
        var quote = Quote.Create(_customerId);
        quote.AddItem(quoteItemData);
        quote.Confirm(currency);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteConfirmed;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteConfirmed>();
    }

    private QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());
    private ProductId _productId = ProductId.Of(Guid.NewGuid());
    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
}