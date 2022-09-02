using EcommerceDDD.Core.Testing;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Quotes.Application.OpeningQuote;

namespace EcommerceDDD.Quotes.Tests.Application;

public class OpenQuoteCommandHandlerTests
{
    [Fact]
    public async Task Open_WithCommand_ShouldCreateQuote()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        _checker.Setup(p => p.CanCustomerOpenNewQuote(customerId))
            .Returns(Task.FromResult(true));

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();

        var command = new OpenQuote(customerId);
        var commandHandler = new OpenQuoteHandler(quoteWriteRepository, _checker.Object);

        // When
        await commandHandler.Handle(command, CancellationToken.None);

        // Then
        quoteWriteRepository.AggregateStream.Should().HaveCount(1);
        var openQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        openQuote.CustomerId.Should().Be(customerId);
        openQuote.Status.Should().Be(QuoteStatus.Open);
    }

    private Mock<ICustomerQuoteOpennessChecker> _checker = new();
}