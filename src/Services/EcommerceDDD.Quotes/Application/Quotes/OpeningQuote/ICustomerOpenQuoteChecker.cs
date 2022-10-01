using EcommerceDDD.Quotes.Domain;

namespace EcommerceDDD.Quotes.Application.Quotes.OpeningQuote;

public interface ICustomerOpenQuoteChecker
{
    Task<bool> CustomerHasOpenQuote(CustomerId customerId);
}