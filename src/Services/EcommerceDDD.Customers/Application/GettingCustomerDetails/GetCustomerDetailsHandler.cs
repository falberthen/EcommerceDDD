using Marten;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Customers.Application.GettingCustomerDetails;

namespace EcommerceDDD.Customers.Api.Application.GettingCustomerDetails;

public class GetCustomerDetailsHandler : QueryHandler<GetCustomerDetails, CustomerDetails>
{
    private readonly TokenIssuerSettings _tokenIssuerSettings;
    private readonly IQuerySession _querySession;
    private readonly IHttpRequester _requester;

    public GetCustomerDetailsHandler(
        IOptions<TokenIssuerSettings> tokenIssuerSettings,
        IHttpRequester httpRequester,
        IQuerySession querySession)
    {
        _tokenIssuerSettings = tokenIssuerSettings.Value;
        _requester = httpRequester;
        _querySession = querySession;
    }

    public override async Task<CustomerDetails> Handle(GetCustomerDetails query, CancellationToken cancellationToken)
    {
        var uri = $"{_tokenIssuerSettings.Authority}/connect/userinfo";

        var response = await _requester
            .GetAsync<UserInfoResponse>(uri, query.UserAccessToken);

        if (response == null)
            throw new ApplicationException("Cannot retrieve customer's user info.");

        var details = new CustomerDetails();
        var customer = _querySession.Query<CustomerDetails>()
            .FirstOrDefault(c => c.Email == response.Email);

        if (customer == null)
            throw new ApplicationException($"Customer not found");

        details.Id = customer.Id;
        details.Email = customer.Email;
        details.Name = customer.Name;
        details.Address = customer.Address;
        details.AvailableCreditLimit = customer.AvailableCreditLimit;

        return details;
    }

    public record class UserInfoResponse(string Email);
}
