namespace EcommerceDDD.QuoteManagement.Application.ConfirmingQuote;

public class ConfirmQuoteHandler(
    IEventStoreRepository<Quote> quoteWriteRepository
)
{
	private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;

	public async Task<Result> HandleAsync(ConfirmQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
			.FetchForWritingAsync(command.QuoteId.Value, cancellationToken: cancellationToken);

        if (quote is null)
            return Result.Fail($"The quote {command.QuoteId} not found.");

        quote.Confirm();

        await _quoteWriteRepository
			.AppendEventsAndCommitAsync(quote, cancellationToken);

        return Result.Ok();
    }
}
