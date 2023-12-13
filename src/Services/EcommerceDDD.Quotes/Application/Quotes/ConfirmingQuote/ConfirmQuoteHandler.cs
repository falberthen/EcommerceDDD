namespace EcommerceDDD.Quotes.Application.Quotes.ConfirmingQuote;

public class ConfirmQuoteHandler : ICommandHandler<ConfirmQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;

    public ConfirmQuoteHandler(        
        IEventStoreRepository<Quote> quoteWriteRepository)
    {
        _quoteWriteRepository = quoteWriteRepository;
    }

    public async Task Handle(ConfirmQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStreamAsync(command.QuoteId.Value)
            ?? throw new RecordNotFoundException($"The quote {command.QuoteId} not found.");
        
        // Quote confirmed
        quote.Confirm(command.Currency);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}