using Marten;
using EcommerceDDD.Customers.Domain;
using EcommerceDDD.Customers.Application.GettingCustomerDetails;

namespace EcommerceDDD.Customers.Application.RegisteringCustomer;

/// <summary>
/// Domain service for checking customer uniqueness
/// </summary>
public class CustomerUniquenessChecker : ICustomerUniquenessChecker
{
    private readonly IQuerySession _querySession;

    public CustomerUniquenessChecker(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public bool IsUserUnique(string customerEmail)
    {
        var customer = _querySession.Query<CustomerDetails>()
            .FirstOrDefault(c => c.Email == customerEmail);

        return customer == null;
    }
}