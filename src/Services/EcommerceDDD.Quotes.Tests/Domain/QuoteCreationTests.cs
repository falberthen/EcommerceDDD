using Moq;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Quotes.Tests.Domain;

public class QuoteCreationTests
{
    [Fact]
    public async Task OpenQuoteForCustomer_WithNoExistingOpenQuote_ReturnsOpenQuote()
    {
        // Given
        _checker.Setup(p => p.CanCustomerOpenNewQuote(_customerId))
            .Returns(Task.FromResult(true));

        // When
        var quote = await Quote.CreateNew(_customerId, _checker.Object);

        // Then
        Assert.NotNull(quote);
        quote.CustomerId.Should().Be(quote.CustomerId);
        quote.CreatedAt.Should().NotBe(null);
        quote.Status.Should().Be(QuoteStatus.Open);
    }

    [Fact]
    public async Task OpenQuoteForCustomer_WithExistingOpenQuote_ThrowsException()
    {
        // Given
        _checker.Setup(p => p.CanCustomerOpenNewQuote(_customerId))
            .Returns(Task.FromResult(false));

        // When
        Func<Task> action = async () =>
           await Quote.CreateNew(_customerId, _checker.Object);

        // Then
        await action.Should().ThrowAsync<DomainException>();
    }

    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
    private Mock<ICustomerQuoteOpennessChecker> _checker = new();
}
