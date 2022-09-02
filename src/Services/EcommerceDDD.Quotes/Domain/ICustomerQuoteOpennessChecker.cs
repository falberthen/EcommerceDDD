namespace EcommerceDDD.Quotes.Domain;

public interface ICustomerQuoteOpennessChecker
{
    Task<bool> CanCustomerOpenNewQuote(CustomerId customerId);
}