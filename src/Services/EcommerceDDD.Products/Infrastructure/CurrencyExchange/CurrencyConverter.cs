namespace EcommerceDDD.Products.Infrastructure.CurrencyConverter;

public class CurrencyConverter : ICurrencyConverter
{
    private Currency _baseCurrency = Currency.USDollar;

    public Currency GetBaseCurrency()
    {
        return _baseCurrency;
    }

    public decimal Convert(decimal value, string currencyCode)
    {
        Currency currency = Currency.OfCode(currencyCode);
        
        if (currency.Code == _baseCurrency.Code)
            return value;

        var conversionRate = GetExchangeRates()
            .SingleOrDefault(x => x.FromCurrency == currency.Code 
            && x.ToCurrency == _baseCurrency.Code);

        // Rate not found
        if (conversionRate is null)
            return value;

        var convertedValue = conversionRate.ConversionRate * value;
        return convertedValue;
    }

    private List<ExchangeRate> GetExchangeRates()
    {
        var conversionRates = new List<ExchangeRate>();

        conversionRates.Add(new ExchangeRate(Currency.USDollar.Code, Currency.CanadianDollar.Code, (decimal)0.76));
        conversionRates.Add(new ExchangeRate(Currency.CanadianDollar.Code, Currency.USDollar.Code, (decimal)1.32));

        conversionRates.Add(new ExchangeRate(Currency.USDollar.Code, Currency.Euro.Code, (decimal)0.84));
        conversionRates.Add(new ExchangeRate(Currency.Euro.Code, Currency.USDollar.Code, (decimal)1.19));

        return conversionRates;
    }
}