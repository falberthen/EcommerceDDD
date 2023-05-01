namespace EcommerceDDD.Customers.Api.Application.GettingCustomerDetails;

public class GetCustomerDetailsHandler : IQueryHandler<GetCustomerDetails, CustomerDetails>
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

    public async Task<CustomerDetails> Handle(GetCustomerDetails query, CancellationToken cancellationToken)
    {
        var uri = $"{_tokenIssuerSettings.Authority}/connect/userinfo";

        var response = await _requester
            .GetAsync<UserInfoResponse>(uri, query.UserAccessToken);

        if (response is null)
            throw new RecordNotFoundException($"Cannot retrieve customer user info.");

        var details = new CustomerDetails();
        var customer = _querySession.Query<CustomerDetails>()
            .FirstOrDefault(c => c.Email == response.Email);

        if (customer is null)
            throw new RecordNotFoundException($"Customer not found.");

        details.Id = customer.Id;
        details.Email = customer.Email;
        details.Name = customer.Name;
        details.ShippingAddress = customer.ShippingAddress;
        details.CreditLimit = customer.CreditLimit;

        return details;
    }

    public record class UserInfoResponse(string Email);
}
