namespace EcommerceDDD.QuoteManagement.Application.Quotes.OpeningQuote;

public interface ICustomerOpenQuoteChecker
{
	Task<QuoteDetails?> CheckCustomerOpenQuoteAsync(CustomerId customerId, CancellationToken cancellationToken);
}