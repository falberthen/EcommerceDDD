namespace EcommerceDDD.QuoteManagement.Tests.Application;

public class RemoveQuoteItemHandlerTests
{
    [Fact]
    public async Task RemoveQuoteItem_WithCommand_ShouldRemoveQuoteItem()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        var productId = ProductId.Of(Guid.NewGuid());
        var productQuantity = 1;
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var openQuote = OpenQuote.Create(customerId, currency);

        var quote = Quote.OpenQuote(openQuote.CustomerId, currency);
        quote.AddItem(new QuoteItemData(quote.Id, productId, "Product",
            Money.Of(10, currency.Code), productQuantity));

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
        await quoteWriteRepository.AppendEventsAsync(quote);

        var command = RemoveQuoteItem.Create(quote.Id, productId);
        var commandHandler = new RemoveQuoteItemHandler(quoteWriteRepository);

        // When
        await commandHandler.Handle(command, CancellationToken.None);

        // Then        
        var storedQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        storedQuote.Items.Count.Should().Be(0);
    }
}