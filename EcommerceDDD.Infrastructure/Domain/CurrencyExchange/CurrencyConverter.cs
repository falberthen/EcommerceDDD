using EcommerceDDD.Domain.CurrencyExchange;
using EcommerceDDD.Domain.Shared;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceDDD.Infrastructure.Domain.ForeignExchanges
{
    public class CurrencyConverter : ICurrencyConverter
    {
        public Currency BaseCurrency = Currency.USDollar;

        public Currency GetBaseCurrency()
        {
            return BaseCurrency;
        }

        public Money Convert(string fromCurrency, Money value)
        {
            var conversionRate = GetExchangeRates()
                .Single(x => x.FromCurrency == fromCurrency && x.ToCurrency == BaseCurrency.Name);

            var convertedValue = conversionRate.ConversionRate * value;
            return convertedValue;
        }

        private List<ExchangeRate> GetExchangeRates()
        {
            var conversionRates = new List<ExchangeRate>();

            conversionRates.Add(new ExchangeRate(Currency.USDollar.Name, Currency.CanadianDollar.Name, (decimal)0.76));
            conversionRates.Add(new ExchangeRate(Currency.CanadianDollar.Name, Currency.USDollar.Name, (decimal)1.32));

            conversionRates.Add(new ExchangeRate(Currency.USDollar.Name, Currency.Euro.Name, (decimal)0.84));
            conversionRates.Add(new ExchangeRate(Currency.Euro.Name, Currency.USDollar.Name, (decimal)1.19));

            return conversionRates;
        }
    }
}