namespace EcommerceDDD.QuoteManagement.Application.CancelingQuote;

public class CancelQuoteHandler(
	IEventStoreRepository<Quote> quoteWriteRepository,
	IUserInfoRequester userInfoRequester
)
{
	private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;
	private readonly IUserInfoRequester _userInfoRequester = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

	public async Task<Result> HandleAsync(CancelQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
			.FetchForWritingAsync(command.QuoteId.Value, cancellationToken: cancellationToken);

        if (quote is null)
            return Result.Fail($"The quote {command.QuoteId.Value} was not found.");

		var ownershipResult = _userInfoRequester
			.EnsureCurrentCustomerOwns(quote.CustomerId.Value);
		if (ownershipResult.IsFailed)
			return ownershipResult;

        quote.Cancel();

        await _quoteWriteRepository
			.AppendEventsAndCommitAsync(quote, cancellationToken);

        return Result.Ok();
    }
}
