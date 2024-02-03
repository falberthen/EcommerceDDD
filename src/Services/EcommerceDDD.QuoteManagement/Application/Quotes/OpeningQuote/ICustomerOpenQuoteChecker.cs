namespace EcommerceDDD.QuoteManagement.Application.Quotes.OpeningQuote;

public interface ICustomerOpenQuoteChecker
{
    Task<bool> CustomerHasOpenQuote(CustomerId customerId);
}