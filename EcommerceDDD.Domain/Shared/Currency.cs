using System;
using System.Collections.Generic;
using EcommerceDDD.Domain.Core.Base;

namespace EcommerceDDD.Domain.Shared
{
    public class Currency : ValueObject
    {
        public string Name { get; }
        public string Symbol { get; }
        public static Currency USDollar => new Currency("USD", "US$");
        public static Currency CanadianDollar => new Currency("CAD", "CA$");
        public static Currency Euro => new Currency("EUR", "€");

        public Currency(string name, string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentNullException(nameof(symbol));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Amount cannot be null or whitespace.", nameof(name));

            Symbol = symbol;
            Name = name;
        }

        public static Currency FromCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            switch (code)
            {
                case "USD":
                    return new Currency(Currency.USDollar.Name, Currency.USDollar.Symbol);                            
                case "CAD":
                    return new Currency(Currency.CanadianDollar.Name, Currency.CanadianDollar.Symbol);
                case "EUR":
                    return new Currency(Currency.Euro.Name, Currency.Euro.Symbol);
                default:
                    throw new ArgumentException($"Invalid code: {code}", nameof(code));
            }
        }

        public static List<string> SupportedCurrencies()
        {
            return new List<string>() { USDollar.Name, Euro.Name, CanadianDollar.Name };
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
            yield return Symbol;
        }

        private Currency() { }
    }
}
