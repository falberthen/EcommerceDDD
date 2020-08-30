namespace EcommerceDDD.Domain.CurrencyExchange
{
    public class ExchangeRate
    {
        public string FromCurrency { get; private set; }
        public string ToCurrency { get; private set; }
        public decimal ConversionRate { get; private set; }

        public ExchangeRate(string sourceCurrency, string targetCurrency, decimal conversionRate)
        {
            FromCurrency = sourceCurrency;
            ToCurrency = targetCurrency;
            ConversionRate = conversionRate;
        }
    }
}