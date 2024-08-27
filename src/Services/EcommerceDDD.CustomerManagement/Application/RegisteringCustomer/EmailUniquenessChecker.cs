namespace EcommerceDDD.CustomerManagement.Application.RegisteringCustomer;

public class EmailUniquenessChecker(IQuerySession querySession) : IEmailUniquenessChecker
{
    private readonly IQuerySession _querySession = querySession;

    public bool IsUnique(string customerEmail)
    {
        var customer = _querySession.Query<CustomerDetails>()
            .FirstOrDefault(c => c.Email == customerEmail);

        return customer is null;
    }
}