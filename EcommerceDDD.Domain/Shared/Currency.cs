using System;
using System.Collections.Generic;
using EcommerceDDD.Domain.Core.Base;

namespace EcommerceDDD.Domain.Shared
{
    public class Currency : ValueObject
    {
        public string Code { get; }
        public string Symbol { get; }
        public static Currency USDollar => new Currency("USD", "US$");
        public static Currency CanadianDollar => new Currency("CAD", "CA$");
        public static Currency Euro => new Currency("EUR", "€");

        public Currency(string code, string symbol)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleException("Code cannot be null or whitespace.");

            if (string.IsNullOrWhiteSpace(symbol))
                throw new BusinessRuleException("Symbol cannot be null or whitespace.");

            Code = code;
            Symbol = symbol;
        }

        public static Currency FromCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            switch (code)
            {
                case "USD":
                    return new Currency(USDollar.Code, USDollar.Symbol);                            
                case "CAD":
                    return new Currency(CanadianDollar.Code, CanadianDollar.Symbol);
                case "EUR":
                    return new Currency(Euro.Code, Euro.Symbol);
                default:
                    throw new BusinessRuleException($"Invalid code {code}");
            }
        }

        public static List<string> SupportedCurrencies()
        {
            return new List<string>() { USDollar.Code, Euro.Code, CanadianDollar.Code };
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Code;
            yield return Symbol;
        }

        private Currency() { }
    }
}
