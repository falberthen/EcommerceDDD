using System.Collections.Generic;
using EcommerceDDD.Domain.Core.Base;

namespace EcommerceDDD.Domain.Shared
{
    public class Money : ValueObject
    {
        public decimal Value { get; }
        public string CurrencyCode { get; }
        public string CurrencySymbol { get; }

        private Money(decimal value, string currencyCode)
        {
            Value = value;
            Currency currency = Currency.FromCode(currencyCode);
            CurrencyCode = currency.Code;
            CurrencySymbol = currency.Symbol;
        }

        public static Money Of(decimal value, string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
                throw new BusinessRuleException("Money must have currency.");

            return new Money(value, currencyCode);
        }

        public static Money operator *(decimal number, Money rightValue)
        {
            return new Money(number * rightValue.Value, rightValue.CurrencyCode);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Value;
            yield return CurrencyCode;
        }

        private Money(){}
    }
}
