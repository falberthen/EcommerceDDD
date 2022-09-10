using EcommerceDDD.Core.Testing;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Application.Quotes.RemovingQuoteItem;
using EcommerceDDD.Quotes.Application.RemovingQuoteItem;

namespace EcommerceDDD.Quotes.Tests.Application;

public class RemoveQuoteItemCommandHandlerTests
{
    [Fact]
    public async Task RemoveQuoteItem_WithCommand_ShouldRemoveQuoteItem()
    {
        // Given
        var _customerId = CustomerId.Of(Guid.NewGuid());
        var productId = ProductId.Of(Guid.NewGuid());
        var _productQuantity = 1;

        var quoteItemData = new QuoteItemData(QuoteId.Of(Guid.NewGuid()), productId, _productQuantity);
        _checker.Setup(p => p.CanCustomerOpenNewQuote(_customerId))
            .Returns(Task.FromResult(true));

        var quote = await Quote.CreateNew(_customerId, _checker.Object);
        quote.AddQuoteItem(quoteItemData);

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
        await quoteWriteRepository.AppendEventsAsync(quote);

        var command = new RemoveQuoteItem(quote.Id, productId);
        var commandHandler = new RemoveQuoteItemHandler(quoteWriteRepository);

        // When
        await commandHandler.Handle(command, CancellationToken.None);

        // Then        
        var storedQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        storedQuote.Items.Count.Should().Be(0);
    }

    private Mock<ICustomerQuoteOpennessChecker> _checker = new();
}