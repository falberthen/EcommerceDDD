using EcommerceDDD.Domain.Core.Base;

namespace EcommerceDDD.Domain.Shared
{
    public class Money : ValueObject<Money>
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

            if (value < 0)
                throw new BusinessRuleException("Money amount value cannot be negative.");

            return new Money(value, currencyCode);
        }

        protected override bool EqualsCore(Money other)
        {
            return Value == other.Value &&
                CurrencyCode == other.CurrencyCode &&
                CurrencySymbol == other.CurrencySymbol;
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                int hashCode = Value.GetHashCode();
                hashCode = (hashCode * 397) ^ CurrencyCode.GetHashCode();
                hashCode = (hashCode * 397) ^ CurrencySymbol.GetHashCode();
                return hashCode;
            }
        }

        public static Money operator *(decimal number, Money rightValue)
        {
            return new Money(number * rightValue.Value, rightValue.CurrencyCode);
        }

        public static Money operator +(Money money1, Money money2)
        {
            if (!money1.CurrencyCode.Equals(money2.CurrencyCode))
                throw new BusinessRuleException("You cannot sum different currencies.");

            return Of(money1.Value + money2.Value, money1.CurrencyCode);            
        }

        private Money(){}
    }
}
