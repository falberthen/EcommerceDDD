namespace EcommerceDDD.Quotes.Tests.Application;

public class CancelQuoteHandlerTests
{
    [Fact]
    public async Task Cancel_WithCommand_ShouldCancelQuote()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        _customerOpenQuoteChecker.CustomerHasOpenQuote(customerId)
            .Returns(Task.FromResult(false));

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();

        var openCommand = OpenQuote.Create(customerId);
        var openCommandHandler = new OpenQuoteHandler(quoteWriteRepository, _customerOpenQuoteChecker);
        await openCommandHandler.Handle(openCommand, CancellationToken.None);

        var quote = quoteWriteRepository.AggregateStream.First().Aggregate;
        var cancelCommand = CancelQuote.Create(quote.Id);
        var cancelCommandHandler = new CancelQuoteHandler(quoteWriteRepository);

        // When
        await cancelCommandHandler.Handle(cancelCommand, CancellationToken.None);

        // Then
        quote.Status.Should().Be(QuoteStatus.Cancelled);
    }

    private ICustomerOpenQuoteChecker _customerOpenQuoteChecker = Substitute.For<ICustomerOpenQuoteChecker>();
}