using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.IntegrationServices.Customers.Responses;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.IntegrationServices.Customers;

public class CustomersService : ICustomersService
{
    private readonly IHttpRequester _httpRequester;
    private readonly ITokenRequester _tokenRequester;
    private readonly TokenIssuerSettings _tokenIssuerSettings;

    public CustomersService(
        IHttpRequester httpRequester,
        ITokenRequester tokenRequester,
        IOptions<TokenIssuerSettings> tokenIssuerSettings)
    {
        _httpRequester = httpRequester;
        _tokenRequester = tokenRequester;
        _tokenIssuerSettings = tokenIssuerSettings.Value;
    }

    public async Task<AvailableCreditLimitModel> RequestAvailableCreditLimit(string apiGatewayUrl, Guid customerId)
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationToken(_tokenIssuerSettings);

        var response = await _httpRequester.GetAsync<AvailableCreditLimitResponse>(
            $"{apiGatewayUrl}/api/customers/{customerId}/credit",
            tokenResponse.AccessToken);

        if (!response.Success)
            throw new Exception("Error retrieving customer available credit limit.");

        return response.Data;
    }
}