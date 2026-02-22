namespace EcommerceDDD.QuoteManagement.Application.OpeningQuote;

public interface ICustomerOpenQuoteChecker
{
	Task<QuoteDetails?> CheckCustomerOpenQuoteAsync(CustomerId customerId, CancellationToken cancellationToken);
}