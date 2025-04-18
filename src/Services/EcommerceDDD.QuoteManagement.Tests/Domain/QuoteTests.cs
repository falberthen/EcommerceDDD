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
        var quote = Quote.OpenQuoteForCustomer(customerId, currency);

        // Then
        Assert.NotNull(quote);
		Assert.NotEqual(default(DateTime), quote.CreatedAt);
		Assert.Equal(quote.CustomerId, quote.CustomerId);
		Assert.Equal(QuoteStatus.Open, quote.Status);
	}
}
