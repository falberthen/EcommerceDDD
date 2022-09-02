using Moq;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Quotes.Domain.Events;

namespace EcommerceDDD.Quotes.Tests.Domain;

public class QuoteEventsTests
{
    [Fact]
    public async Task CreatingQuote_ReturnsQuoteCreatedEvent()
    {
        // Given
        _checker.Setup(p => p.CanCustomerOpenNewQuote(_customerId))
            .Returns(Task.FromResult(true));

        // When
        var quote = await Quote.CreateNew(_customerId, _checker.Object);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteCreated;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteCreated>();
    }

    [Fact]
    public async Task CancellingQuote_ReturnsQuoteCancelledEvent()
    {
        // Given
        _checker.Setup(p => p.CanCustomerOpenNewQuote(_customerId))
            .Returns(Task.FromResult(true));

        // When
        var quote = await Quote.CreateNew(_customerId, _checker.Object);
        quote.Cancel();

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteCancelled;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteCancelled>();
    }

    [Fact]
    public async Task AddingItemToQuote_ReturnsQuoteItemAddedEvent()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        _checker.Setup(p => p.CanCustomerOpenNewQuote(customerId))
            .Returns(Task.FromResult(true));

        // When
        var quote = await Quote.CreateNew(customerId, _checker.Object);
        quote.AddQuoteItem(new QuoteItemData(Guid.NewGuid(), QuoteId.Of(Guid.NewGuid()), ProductId.Of(Guid.NewGuid()), 1));

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteItemAdded;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteItemAdded>();
    }

    [Fact]
    public async Task RemovingItemFromQuote_ReturnsQuoteItemRemovedEvent()
    {
        // Given
        var quoteItemData = new QuoteItemData(Guid.NewGuid(), QuoteId.Of(Guid.NewGuid()), _productId, 1);

        _checker.Setup(p => p.CanCustomerOpenNewQuote(_customerId))
            .Returns(Task.FromResult(true));

        // When
        var quote = await Quote.CreateNew(_customerId, _checker.Object);
        quote.AddQuoteItem(quoteItemData);
        quote.RemoveQuoteItem(_productId);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteItemRemoved;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteItemRemoved>();
    }

    [Fact]
    public async Task ChangingQuoteItemQuantity_ReturnsQuoteItemQuantityChangedEvent()
    {
        // Given
        var quoteItemData = new QuoteItemData(Guid.NewGuid(), QuoteId.Of(Guid.NewGuid()), _productId, 1);
        _checker.Setup(p => p.CanCustomerOpenNewQuote(_customerId))
            .Returns(Task.FromResult(true));

        // When
        var quote = await Quote.CreateNew(_customerId, _checker.Object);
        quote.AddQuoteItem(quoteItemData);
        quoteItemData = quoteItemData with { Quantity = 2 };
        quote.AddQuoteItem(quoteItemData);

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteItemQuantityChanged;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteItemQuantityChanged>();
    }

    [Fact]
    public async Task ConfirmingQuote_ReturnsQuoteConfirmedEvent()
    {
        // Given
        var quoteItemData = new QuoteItemData(Guid.NewGuid(), QuoteId.Of(Guid.NewGuid()), _productId, 1);
        _checker.Setup(p => p.CanCustomerOpenNewQuote(_customerId))
            .Returns(Task.FromResult(true));

        // When
        var quote = await Quote.CreateNew(_customerId, _checker.Object);
        quote.AddQuoteItem(quoteItemData);
        quote.Confirm();

        // Then
        var @event = quote.GetUncommittedEvents().LastOrDefault() as QuoteConfirmed;
        Assert.NotNull(@event);
        @event.Should().BeOfType<QuoteConfirmed>();
    }

    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
    private ProductId _productId = ProductId.Of(Guid.NewGuid());
    private Mock<ICustomerQuoteOpennessChecker> _checker = new();
}