namespace EcommerceDDD.CustomerManagement.Application.RegisteringCustomer;

public class EmailUniquenessChecker(IQuerySession querySession) : IEmailUniquenessChecker
{
    private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

    public async Task<bool> IsUniqueAsync(string customerEmail, CancellationToken cancellationToken)
    {
        var customer = await _querySession.Query<CustomerDetails>()
            .SingleOrDefaultAsync(c => c.Email == customerEmail, token: cancellationToken);

        return customer is null;
    }
}