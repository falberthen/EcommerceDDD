using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Payments.Domain;

public class Currency : ValueObject<Currency>
{
    public string Code { get; }
    public string Symbol { get; }
    public static Currency USDollar => new Currency("USD", "$");
    public static Currency CanadianDollar => new Currency("CAD", "CDN$");
    public static Currency Euro => new Currency("EUR", "€");

    public Currency(string code, string symbol)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Code cannot be null or whitespace.");

        if (string.IsNullOrWhiteSpace(symbol))
            throw new DomainException("Symbol cannot be null or whitespace.");

        Code = code;
        Symbol = symbol;
    }

    public static Currency OfCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Code cannot be null or whitespace.");

        return code switch
        {
            "USD" => new Currency(USDollar.Code, USDollar.Symbol),
            "CAD" => new Currency(CanadianDollar.Code, CanadianDollar.Symbol),
            "EUR" => new Currency(Euro.Code, Euro.Symbol),
            _ => throw new DomainException($"Invalid code {code}")
        };
    }

    public static List<string> SupportedCurrencies()
    {
        return new List<string>() { USDollar.Code, Euro.Code, CanadianDollar.Code };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
        yield return Symbol;
    }

    private Currency() { }
}