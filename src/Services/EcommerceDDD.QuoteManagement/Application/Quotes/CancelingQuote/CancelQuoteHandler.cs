namespace EcommerceDDD.QuoteManagement.Application.Quotes.CancelingQuote;

public class CancelQuoteHandler(
	IEventStoreRepository<Quote> quoteWriteRepository
) : ICommandHandler<CancelQuote>
{
	private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;

	public async Task HandleAsync(CancelQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
			.FetchStreamAsync(command.QuoteId.Value, cancellationToken: cancellationToken)
            ?? throw new RecordNotFoundException($"The quote {command.QuoteId} was not found.");
     
        quote.Cancel();

        await _quoteWriteRepository
			.AppendEventsAsync(quote, cancellationToken);
    }
}