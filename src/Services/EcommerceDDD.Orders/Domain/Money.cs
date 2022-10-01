using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Orders.Domain;

public class Money : ValueObject<Money>
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public static Money Of(decimal value, string currencyCode)
    {
        if (string.IsNullOrEmpty(currencyCode))
            throw new BusinessRuleException("Money must have currency.");

        if (value < 0)
            throw new BusinessRuleException("Money amount value cannot be negative.");

        return new Money(value, currencyCode);
    }

    public static Money operator *(decimal number, Money rightValue)
    {
        return new Money(number * rightValue.Amount, rightValue.Currency.Code);
    }

    public static Money operator +(Money money1, Money money2)
    {
        if (!money1.Currency.Code.Equals(money2.Currency.Code))
            throw new BusinessRuleException("You cannot sum different currencies.");

        return Of(money1.Amount + money2.Amount, money1.Currency.Code);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    private Money(decimal amount, string currencyCode)
    {
        Amount = amount;
        Currency = Currency.OfCode(currencyCode);
    }

    private Money() {}
}