using EcommerceDDD.Products.Domain;

public interface ICurrencyConverter
{
    Currency GetBaseCurrency();
    decimal Convert(decimal value, string currencyCode);
}