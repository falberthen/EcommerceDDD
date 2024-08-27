namespace EcommerceDDD.QuoteManagement.Application.Quotes.OpeningQuote;

public class OpenQuoteHandler(IEventStoreRepository<
        Quote> quoteWriteRepository,
    ICustomerOpenQuoteChecker customerOpenQuoteChecker) : ICommandHandler<OpenQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;
    private readonly ICustomerOpenQuoteChecker _customerOpenQuoteChecker = customerOpenQuoteChecker;

    public async Task Handle(OpenQuote command, CancellationToken cancellationToken)
    {
        if (await _customerOpenQuoteChecker.CustomerHasOpenQuote(command.CustomerId))
            throw new ApplicationLogicException(
                $"The customer {command.CustomerId.Value} has an open quote already.");

        var quote = Quote.OpenQuote(command.CustomerId, command.Currency);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}