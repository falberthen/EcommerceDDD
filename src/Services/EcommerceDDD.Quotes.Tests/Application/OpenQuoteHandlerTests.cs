namespace EcommerceDDD.Quotes.Tests.Application;

public class OpenQuoteHandlerTests
{
    [Fact]
    public async Task OpenQuote_WithCommand_ShouldCreateQuote()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        _customerOpenQuoteChecker.CustomerHasOpenQuote(customerId)
            .Returns(Task.FromResult(false));

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();

        var command = OpenQuote.Create(customerId);
        var commandHandler = new OpenQuoteHandler(quoteWriteRepository, _customerOpenQuoteChecker);

        // When
        await commandHandler.Handle(command, CancellationToken.None);

        // Then
        quoteWriteRepository.AggregateStream.Should().HaveCount(1);
        var openQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        openQuote.CustomerId.Should().Be(customerId);
        openQuote.Status.Should().Be(QuoteStatus.Open);
    }

    private ICustomerOpenQuoteChecker _customerOpenQuoteChecker = Substitute.For<ICustomerOpenQuoteChecker>();
}