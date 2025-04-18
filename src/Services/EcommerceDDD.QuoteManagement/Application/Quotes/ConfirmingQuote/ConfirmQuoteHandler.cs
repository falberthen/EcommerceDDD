namespace EcommerceDDD.QuoteManagement.Application.Quotes.ConfirmingQuote;

public class ConfirmQuoteHandler(
    IEventStoreRepository<Quote> quoteWriteRepository
) : ICommandHandler<ConfirmQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;

    public async Task HandleAsync(ConfirmQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStreamAsync(command.QuoteId.Value)
            ?? throw new RecordNotFoundException($"The quote {command.QuoteId} not found.");
        
        // Quote confirmed
        quote.Confirm();

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}