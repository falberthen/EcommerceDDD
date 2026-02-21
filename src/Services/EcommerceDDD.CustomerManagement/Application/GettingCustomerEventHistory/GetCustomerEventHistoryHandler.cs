namespace EcommerceDDD.CustomerManagement.Application.GettingCustomerEventHistory;

public class GetCustomerEventHistoryHandler(
	IUserInfoRequester userInfoRequester,
	IQuerySession querySession
) : IQueryHandler<GetCustomerEventHistory, IReadOnlyList<CustomerEventHistory>>
{
	private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));
	private IUserInfoRequester _userInfoRequester { get; set; } = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

	public async Task<Result<IReadOnlyList<CustomerEventHistory>>> HandleAsync(GetCustomerEventHistory query,
		CancellationToken cancellationToken)
	{
		UserInfo? userInfo = await _userInfoRequester.RequestUserInfoAsync();

		if (userInfo is null)
			return Result.Fail<IReadOnlyList<CustomerEventHistory>>(
				new RecordNotFoundError($"Customer not found."));

		var customerHistory = await _querySession.Query<CustomerEventHistory>()
		   .Where(c => c.AggregateId == userInfo.CustomerId)
		   .OrderBy(c => c.Timestamp)
		   .ToListAsync(cancellationToken);

		return Result.Ok(customerHistory);
	}
}
