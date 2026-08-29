namespace EcommerceDDD.QuoteManagement.Application.GettingQuoteHistory;

public class GetQuoteEventHistoryHandler(
	IQuerySession querySession,
	IUserInfoRequester userInfoRequester
)
{
    private readonly IQuerySession _querySession = querySession;
	private readonly IUserInfoRequester _userInfoRequester = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

    public async Task<Result<IReadOnlyList<QuoteEventHistory>>> HandleAsync(GetQuoteEventHistory query, CancellationToken cancellationToken)
    {
		var quoteDetails = await _querySession.Query<QuoteDetails>()
			.FirstOrDefaultAsync(q => q.Id == query.QuoteId.Value, token: cancellationToken);

		if (quoteDetails is null)
			return Result.Fail<IReadOnlyList<QuoteEventHistory>>(
				new RecordNotFoundError($"The quote {query.QuoteId.Value} was not found."));

		var ownershipResult = _userInfoRequester
			.EnsureCurrentCustomerOwns(quoteDetails.CustomerId);
		if (ownershipResult.IsFailed)
			return Result.Fail<IReadOnlyList<QuoteEventHistory>>(ownershipResult.Errors);

		var quoteHistory = await _querySession.Query<QuoteEventHistory>()
		   .Where(c => c.AggregateId == query.QuoteId.Value)
		   .OrderBy(c => c.Timestamp)
			.ToListAsync(cancellationToken);

		return Result.Ok<IReadOnlyList<QuoteEventHistory>>(quoteHistory);
    }
}
