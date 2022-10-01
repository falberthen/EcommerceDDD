using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Products.Domain;

namespace EcommerceDDD.Products.Tests;

public class MoneyTests
{
    [Fact]
    public void TwoOfMoney_WithSameValues_ShouldBeEqual()
    {
        // Given
        Money money1 = Money.Of(10, Currency.USDollar.Code);
        Money money2 = Money.Of(10, Currency.USDollar.Code);

        // Then
        money1.Should().Be(money2);
        money1.GetHashCode().Should().Be(money2.GetHashCode());
    }

    [Fact]
    public void TwoOfMoney_WithDifferentValues_ShouldNotBeEqual()
    {
        // Given
        Money money1 = Money.Of(20, Currency.USDollar.Code);
        Money money2 = Money.Of(10, Currency.USDollar.Code);

        // Then
        money1.Should().NotBe(money2);
        money1.GetHashCode().Should().NotBe(money2.GetHashCode());
    }

    [Fact]
    public void CreateOfMoney_WithNegativeAmount_ShouldThrowException()
    {
        // Given
        Action action = () =>
            Money.Of(-20, Currency.USDollar.Code); // When 

        // Then
        action.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void SumTwoOfMoney_WithDifferentCurrencies_ShouldThrowException()
    {
        // Given
        Money money1 = Money.Of(10, Currency.USDollar.Code);
        Money money2 = Money.Of(10, Currency.CanadianDollar.Code);

        // When
        var ex = Assert.Throws<BusinessRuleException>(() => money1 + money2);

        // Then
        ex.GetType().Should().Be(typeof(BusinessRuleException));
    }

    [Fact]
    public void SumTwoOfMoney_WithSameCurrencies_ShouldSumAmounts()
    {
        // Given
        Money money1 = Money.Of(10, Currency.USDollar.Code);
        Money money2 = Money.Of(10, Currency.USDollar.Code);

        // When
        var total = (money1 + money2);

        // Then
        total.Amount.Should().Be(20);
        total.Currency.Symbol.Should().Be(Currency.USDollar.Symbol);
    }

    [Fact]
    public void OfMoney_MultipliedByDecimal_ShouldMultiplyAmounts()
    {
        // Given
        decimal quantity = 15;
        Money money2 = Money.Of(10, Currency.USDollar.Code);

        // When
        var total = (quantity * money2);

        // Then
        total.Amount.Should().Be(150);
        total.Currency.Symbol.Should().Be(Currency.USDollar.Symbol);
    }
}