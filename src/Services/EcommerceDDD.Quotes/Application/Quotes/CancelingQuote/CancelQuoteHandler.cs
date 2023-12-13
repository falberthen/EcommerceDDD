namespace EcommerceDDD.Quotes.Application.Quotes.CancelingQuote;

public class CancelQuoteHandler : ICommandHandler<CancelQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;

    public CancelQuoteHandler(IEventStoreRepository<Quote> quoteWriteRepository)
    {
        _quoteWriteRepository = quoteWriteRepository;
    }

    public async Task Handle(CancelQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStreamAsync(command.QuoteId.Value)
            ?? throw new RecordNotFoundException($"The quote {command.QuoteId} was not found.");
     
        quote.Cancel();

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}