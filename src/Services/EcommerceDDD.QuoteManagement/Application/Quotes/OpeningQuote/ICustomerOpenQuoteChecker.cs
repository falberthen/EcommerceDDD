namespace EcommerceDDD.QuoteManagement.Application.Quotes.OpeningQuote;

public interface ICustomerOpenQuoteChecker
{
	QuoteDetails? CheckCustomerOpenQuote(CustomerId customerId);
}