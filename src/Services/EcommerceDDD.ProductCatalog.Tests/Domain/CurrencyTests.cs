namespace EcommerceDDD.ProductCatalog.Tests;

public class CurrencyTests
{
	[Fact]
	public void CreateCurrency_WithOfCode_ShouldReturnCurrency()
	{
		// Given
		var code = Currency.USDollar.Code;

		// When
		Currency currency = Currency.OfCode(code);

		// Then        
		Assert.NotNull(currency);
		Assert.Equal(code, currency.Code);
	}

	[Fact]
	public void CreateCurrency_WithEmptyCode_ShouldThrowException()
	{
		// Given
		var code = string.Empty;

		// When & Then
		Assert.Throws<BusinessRuleException>(() =>
			Currency.OfCode(code));
	}

	[Fact]
	public void CreateCurrency_WithInvalidCode_ShouldThrowException()
	{
		// Given
		var code = "USK";

		// When & Then
		Assert.Throws<BusinessRuleException>(() =>
			Currency.OfCode(code));
	}
}