namespace EcommerceDDD.OrderProcessing.Application.GettingOrderEventHistory;

public class GetOrderEventHistoryHandler(
    IQuerySession querySession,
    IUserInfoRequester userInfoRequester
) : IQueryHandler<GetOrderEventHistory, IReadOnlyList<OrderEventHistory>>
{
	private readonly IQuerySession _querySession = querySession;
	private readonly IUserInfoRequester _userInfoRequester = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

	public async Task<Result<IReadOnlyList<OrderEventHistory>>> HandleAsync(GetOrderEventHistory query,
        CancellationToken cancellationToken)
    {
		var orderDetails = await _querySession.Query<OrderDetails>()
			.FirstOrDefaultAsync(o => o.Id == query.OrderId.Value, token: cancellationToken);

		if (orderDetails is null)
			return Result.Fail<IReadOnlyList<OrderEventHistory>>(
				new RecordNotFoundError($"The order {query.OrderId.Value} was not found."));

		var ownershipResult = _userInfoRequester
			.EnsureCurrentCustomerOwns(orderDetails.CustomerId);
		if (ownershipResult.IsFailed)
			return Result.Fail<IReadOnlyList<OrderEventHistory>>(ownershipResult.Errors);

        var quoteHistory = await _querySession.Query<OrderEventHistory>()
           .Where(c => c.AggregateId == query.OrderId.Value)
           .OrderBy(c => c.Timestamp)
           .ToListAsync(cancellationToken);

        return Result.Ok(quoteHistory);
    }
}
