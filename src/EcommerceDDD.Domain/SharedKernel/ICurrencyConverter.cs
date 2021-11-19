namespace EcommerceDDD.Domain.SharedKernel;

public interface ICurrencyConverter
{
    Currency GetBaseCurrency();
    Money Convert(Currency currency, Money value);
}