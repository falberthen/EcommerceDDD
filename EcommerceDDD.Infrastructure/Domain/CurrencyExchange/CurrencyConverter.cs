using EcommerceDDD.Domain.Services;
using EcommerceDDD.Domain.Shared;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceDDD.Infrastructure.Domain.CurrencyExchange
{
    public class CurrencyConverter : ICurrencyConverter
    {
        public Currency BaseCurrency = Currency.USDollar;

        public Currency GetBaseCurrency()
        {
            return BaseCurrency;
        }

        public Money Convert(Currency currency, Money value)
        {
            //Do not convert 
            if (currency.Code == BaseCurrency.Code)
                return Money.Of(value.Value, currency.Code);

            var conversionRate = GetExchangeRates()
                .SingleOrDefault(x => x.FromCurrency == currency.Code && x.ToCurrency == BaseCurrency.Code);

            // Rate not found
            if(conversionRate == null)
                return value;

            var convertedValue = conversionRate.ConversionRate * value;
            return Money.Of(convertedValue.Value, currency.Code);
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
}