namespace EcommerceDDD.QuoteManagement.Tests.Domain;

public class QuoteTests
{
    [Fact]
    public void OpenQuote_WithCustomerIdAndCurrency_ShouldOpenQuote()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);

        // When
        var quote = Quote.OpenQuote(customerId, currency);

        // Then
        Assert.NotNull(quote);
        quote.CustomerId.Should().Be(quote.CustomerId);
        quote.CreatedAt.Should().NotBe(null);
        quote.Status.Should().Be(QuoteStatus.Open);
    }
}
