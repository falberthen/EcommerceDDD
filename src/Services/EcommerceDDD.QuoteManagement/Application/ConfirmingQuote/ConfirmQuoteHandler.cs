namespace EcommerceDDD.QuoteManagement.Application.ConfirmingQuote;

public class ConfirmQuoteHandler(
    IEventStoreRepository<Quote> quoteWriteRepository
) : ICommandHandler<ConfirmQuote>
{
	private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;

	public async Task<Result> HandleAsync(ConfirmQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
			.FetchStreamAsync(command.QuoteId.Value, cancellationToken: cancellationToken);

        if (quote is null)
            return Result.Fail($"The quote {command.QuoteId} not found.");

        quote.Confirm();

        await _quoteWriteRepository
			.AppendEventsAsync(quote, cancellationToken);

        return Result.Ok();
    }
}
