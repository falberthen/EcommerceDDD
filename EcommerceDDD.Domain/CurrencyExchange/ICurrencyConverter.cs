using EcommerceDDD.Domain.Shared;

namespace EcommerceDDD.Domain.CurrencyExchange
{
    public interface ICurrencyConverter
    {
        Currency GetBaseCurrency();
        Money Convert(string fromCurrency, Money value);
    }
}