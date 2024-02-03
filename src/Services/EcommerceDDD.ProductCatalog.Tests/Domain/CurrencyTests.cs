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
        currency.Should().NotBe(null);
        currency.Code.Should().Be(code);
    }

    [Fact]
    public void CreateCurrency_WithEmptyCode_ShouldThrowException()
    {
        // Given
        var code = string.Empty;

        // When
        Action action = () =>
            Currency.OfCode(code);

        // Then
        action.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void CreateCurrency_WithInvalidCode_ShouldThrowException()
    {
        // Given
        var code = "USK";

        // When
        Action action = () =>
            Currency.OfCode(code);

        // Then
        action.Should().Throw<BusinessRuleException>();
    }
}