using System;
using System.Collections.Generic;
using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.SharedKernel
{
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

            return code switch
            {
                "USD" => new Currency(USDollar.Code, USDollar.Symbol),
                "CAD" => new Currency(CanadianDollar.Code, CanadianDollar.Symbol),
                "EUR" => new Currency(Euro.Code, Euro.Symbol),
                _ => throw new BusinessRuleException($"Invalid code {code}")
            };
        }

        public static List<string> SupportedCurrencies()
        {
            return new List<string>() { USDollar.Code, Euro.Code, CanadianDollar.Code };
        }

        protected override bool EqualsCore(Currency other)
        {
            return Code == other.Code && Symbol == other.Symbol;
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                int hashCode = Code.GetHashCode();
                hashCode = (hashCode * 397) ^ Symbol.GetHashCode();
                return hashCode;
            }
        }

        private Currency() { }
    }
}
