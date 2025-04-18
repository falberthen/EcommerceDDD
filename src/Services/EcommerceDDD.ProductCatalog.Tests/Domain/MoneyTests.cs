namespace EcommerceDDD.ProductCatalog.Tests;

public class MoneyTests
{
	[Fact]
	public void TwoOfMoney_WithSameValues_ShouldBeEqual()
	{
		// Given
		Money money1 = Money.Of(10, Currency.USDollar.Code);
		Money money2 = Money.Of(10, Currency.USDollar.Code);

		// Then
		Assert.Equal(money1, money2);
		Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
	}

	[Fact]
	public void TwoOfMoney_WithDifferentValues_ShouldNotBeEqual()
	{
		// Given
		Money money1 = Money.Of(20, Currency.USDollar.Code);
		Money money2 = Money.Of(10, Currency.USDollar.Code);

		// Then
		Assert.NotEqual(money1, money2);
		Assert.NotEqual(money1.GetHashCode(), money2.GetHashCode());
	}

	[Fact]
	public void CreateOfMoney_WithNegativeAmount_ShouldThrowException()
	{
		// Given & When & Then
		Assert.Throws<BusinessRuleException>(() =>
			Money.Of(-20, Currency.USDollar.Code));
	}

	[Fact]
	public void SumTwoOfMoney_WithDifferentCurrencies_ShouldThrowException()
	{
		// Given
		Money money1 = Money.Of(10, Currency.USDollar.Code);
		Money money2 = Money.Of(10, Currency.CanadianDollar.Code);

		// When & Then
		BusinessRuleException ex = Assert
			.Throws<BusinessRuleException>(() => money1 + money2);
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
		Assert.Equal(20, total.Amount);
		Assert.Equal(total.Currency.Symbol, Currency.USDollar.Symbol);
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
		Assert.Equal(150, total.Amount);
		Assert.Equal(total.Currency.Symbol, Currency.USDollar.Symbol);
	}
}