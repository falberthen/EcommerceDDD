using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Customers.Infrastructure.Projections;

namespace EcommerceDDD.Customers.Api.Application.GettingCustomerDetails;

public record class GetCustomerDetails : IQuery<CustomerDetails>
{
    public string UserAccessToken { get; private set; }

    public static GetCustomerDetails Create(string userAccessToken)
    {
        if (string.IsNullOrEmpty(userAccessToken))
            throw new ArgumentNullException(nameof(userAccessToken));

        return new GetCustomerDetails(userAccessToken);
    }

    private GetCustomerDetails(string userAccessToken)
    {
        UserAccessToken = userAccessToken;
    }
}