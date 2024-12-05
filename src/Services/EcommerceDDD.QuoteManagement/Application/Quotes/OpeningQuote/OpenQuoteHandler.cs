namespace EcommerceDDD.QuoteManagement.Application.Quotes.OpeningQuote;

public class OpenQuoteHandler(IEventStoreRepository<Quote> quoteWriteRepository,
    ICustomerOpenQuoteChecker customerOpenQuoteChecker) : ICommandHandler<OpenQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;
    private readonly ICustomerOpenQuoteChecker _customerOpenQuoteChecker = customerOpenQuoteChecker;

    public async Task Handle(OpenQuote command, CancellationToken cancellationToken)
    {
		QuoteDetails? openQuote = _customerOpenQuoteChecker
			.CheckCustomerOpenQuote(command.CustomerId);

		if (openQuote is not null)
            throw new ApplicationLogicException(
                $"The customer {command.CustomerId.Value} has quote {openQuote.Id} open already.");

        var quote = Quote.OpenQuote(command.CustomerId, command.Currency);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}