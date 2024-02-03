namespace EcommerceDDD.CustomerManagement.Api.Application.GettingCustomerDetailsWithToken;

public record class GetCustomerDetailsWithToken : IQuery<CustomerDetails>
{
    public string UserAccessToken { get; private set; }

    public static GetCustomerDetailsWithToken Create(string userAccessToken)
    {
        if (string.IsNullOrEmpty(userAccessToken))
            throw new ArgumentNullException(nameof(userAccessToken));

        return new GetCustomerDetailsWithToken(userAccessToken);
    }

    private GetCustomerDetailsWithToken(string userAccessToken)
    {
        UserAccessToken = userAccessToken;
    }
}