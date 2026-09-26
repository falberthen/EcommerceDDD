namespace EcommerceDDD.CustomerManagement.Application.RegisteringCustomer;

public class CustomerEmailLookup(IQuerySession querySession) : ICustomerEmailLookup
{
    private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

    public async Task<Guid?> FindCustomerIdAsync(string customerEmail, CancellationToken cancellationToken)
    {
        var customer = await _querySession.Query<CustomerDetails>()
            .SingleOrDefaultAsync(c => c.Email == customerEmail, token: cancellationToken);

        return customer?.Id;
    }
}
