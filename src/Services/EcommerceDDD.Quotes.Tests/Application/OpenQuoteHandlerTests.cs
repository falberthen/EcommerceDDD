using EcommerceDDD.Core.Testing;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Quotes.Application.Quotes.OpeningQuote;
using EcommerceDDD.Quotes.Domain.Commands;

namespace EcommerceDDD.Quotes.Tests.Application;

public class OpenQuoteHandlerTests
{
    [Fact]
    public async Task Open_WithCommand_ShouldCreateQuote()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        _customerOpenQuoteChecker.Setup(p => p.CustomerHasOpenQuote(customerId))
            .Returns(Task.FromResult(false));

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();

        var command = OpenQuote.Create(customerId);
        var commandHandler = new OpenQuoteHandler(quoteWriteRepository, _customerOpenQuoteChecker.Object);

        // When
        await commandHandler.Handle(command, CancellationToken.None);

        // Then
        quoteWriteRepository.AggregateStream.Should().HaveCount(1);
        var openQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        openQuote.CustomerId.Should().Be(customerId);
        openQuote.Status.Should().Be(QuoteStatus.Open);
    }

    private Mock<ICustomerOpenQuoteChecker> _customerOpenQuoteChecker = new();
}