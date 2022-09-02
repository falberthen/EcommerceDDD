using EcommerceDDD.Core.Testing;
using EcommerceDDD.Quotes.Application.OpeningQuote;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Application.Quotes.CancelingQuote;

namespace EcommerceDDD.Quotes.Tests.Application;

public class CancelQuoteCommandHandlerTests
{
    [Fact]
    public async Task Cancel_WithCommand_ShouldCancelQuote()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        _checker.Setup(p => p.CanCustomerOpenNewQuote(customerId))
            .Returns(Task.FromResult(true));

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();

        var openCommand = new OpenQuote(customerId);
        var openCommandHandler = new OpenQuoteHandler(quoteWriteRepository, _checker.Object);
        await openCommandHandler.Handle(openCommand, CancellationToken.None);

        var quote = quoteWriteRepository.AggregateStream.First().Aggregate;
        var cancelCommand = new CancelQuote(quote.Id);
        var cancelCommandHandler = new CancelQuoteHandler(quoteWriteRepository);

        // When
        await cancelCommandHandler.Handle(cancelCommand, CancellationToken.None);

        // Then
        quote.Status.Should().Be(QuoteStatus.Cancelled);
    }

    private Mock<ICustomerQuoteOpennessChecker> _checker = new();
}