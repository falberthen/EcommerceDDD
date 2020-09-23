using EcommerceDDD.Domain.Shared;

namespace EcommerceDDD.Domain.Services
{
    public interface ICurrencyConverter
    {
        Currency GetBaseCurrency();
        Money Convert(Currency currency, Money value);
    }
}