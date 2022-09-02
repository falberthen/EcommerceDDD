using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Products.Domain;

public class Money : ValueObject<Money>
{
    public decimal Value { get; }
    public string CurrencyCode { get; }
    public string CurrencySymbol { get; }

    public static Money Of(decimal value, string currencyCode)
    {
        if (string.IsNullOrEmpty(currencyCode))
            throw new DomainException("Money must have currency.");

        if (value < 0)
            throw new DomainException("Money amount value cannot be negative.");

        return new Money(value, currencyCode);
    }

    public static Money operator *(decimal number, Money rightValue)
    {
        return new Money(number * rightValue.Value, rightValue.CurrencyCode);
    }

    public static Money operator +(Money money1, Money money2)
    {
        if (!money1.CurrencyCode.Equals(money2.CurrencyCode))
            throw new DomainException("You cannot sum different currencies.");

        return Of(money1.Value + money2.Value, money1.CurrencyCode);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return CurrencyCode;
        yield return CurrencySymbol;
    }

    private Money(decimal value, string currencyCode)
    {
        Value = value;
        Currency currency = Currency.OfCode(currencyCode);
        CurrencyCode = currency.Code;
        CurrencySymbol = currency.Symbol;
    }
}