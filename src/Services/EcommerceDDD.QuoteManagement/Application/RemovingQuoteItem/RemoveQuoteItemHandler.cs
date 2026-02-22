namespace EcommerceDDD.QuoteManagement.Application.RemovingQuoteItem;

public class RemoveQuoteItemHandler(
	IEventStoreRepository<Quote> quoteWriteRepository
) : ICommandHandler<RemoveQuoteItem>
{
	private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;

	public async Task<Result> HandleAsync(RemoveQuoteItem command, CancellationToken cancellationToken)
	{
		var quote = await _quoteWriteRepository
			.FetchStreamAsync(command.QuoteId.Value, cancellationToken: cancellationToken);

		if (quote is null)
			return Result.Fail($"The quote {command.QuoteId} not found.");

		quote.RemoveItem(command.ProductId);

		await _quoteWriteRepository
			.AppendEventsAsync(quote, cancellationToken);

		return Result.Ok();
	}
}
