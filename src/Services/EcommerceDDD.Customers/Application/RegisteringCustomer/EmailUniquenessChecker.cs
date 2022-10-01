using Marten;
using EcommerceDDD.Customers.Domain;
using EcommerceDDD.Customers.Infrastructure.Projections;

namespace EcommerceDDD.Customers.Application.RegisteringCustomer;

public class EmailUniquenessChecker : IEmailUniquenessChecker
{
    private readonly IQuerySession _querySession;

    public EmailUniquenessChecker(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public bool IsUnique(string customerEmail)
    {
        var customer = _querySession.Query<CustomerDetails>()
            .FirstOrDefault(c => c.Email == customerEmail);

        return customer is null;
    }
}