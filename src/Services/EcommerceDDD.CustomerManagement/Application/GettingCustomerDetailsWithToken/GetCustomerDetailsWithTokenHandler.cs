namespace EcommerceDDD.CustomerManagement.Api.Application.GettingCustomerDetailsWithToken;

public class GetCustomerDetailsWithTokenHandler(
    IOptions<TokenIssuerSettings> tokenIssuerSettings,
    IHttpRequester httpRequester,
    IQuerySession querySession) : IQueryHandler<GetCustomerDetailsWithToken, CustomerDetails>
{
    private readonly TokenIssuerSettings _tokenIssuerSettings = tokenIssuerSettings.Value;
    private readonly IQuerySession _querySession = querySession;
    private readonly IHttpRequester _requester = httpRequester;

    public async Task<CustomerDetails> Handle(GetCustomerDetailsWithToken query, 
        CancellationToken cancellationToken)
    {
        var uri = $"{_tokenIssuerSettings.Authority}/connect/userinfo";

        var response = await _requester
            .GetAsync<UserInfoResponse>(uri, query.UserAccessToken)
            ?? throw new RecordNotFoundException($"Cannot retrieve customer user info.");

        var details = new CustomerDetails();
        var customer = _querySession.Query<CustomerDetails>()
            .FirstOrDefault(c => c.Email == response.Email)
            ?? throw new RecordNotFoundException($"Customer not found.");

        details.Id = customer.Id;
        details.Email = customer.Email;
        details.Name = customer.Name;
        details.ShippingAddress = customer.ShippingAddress;
        details.CreditLimit = customer.CreditLimit;

        return details;
    }

    public record class UserInfoResponse(string Email);
}
