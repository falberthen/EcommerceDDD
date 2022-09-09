using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.IntegrationServices.Base;
using EcommerceDDD.IntegrationServices.Payments.Requests;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.IntegrationServices.Payments;

public class PaymentsService : IPaymentsService
{
    private readonly IHttpRequester _httpRequester;
    private readonly ITokenRequester _tokenRequester;
    private readonly TokenIssuerSettings _tokenIssuerSettings;

    public PaymentsService(
        IHttpRequester httpRequester,
        ITokenRequester tokenRequester,
        IOptions<TokenIssuerSettings> tokenIssuerSettings)
    {
        _httpRequester = httpRequester;
        _tokenRequester = tokenRequester;
        _tokenIssuerSettings = tokenIssuerSettings.Value;
    }

    public async Task<IntegrationServiceResponse> CancelPayment(string apiGatewayUrl, Guid paymentId, CancelPaymentRequest request)
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationToken(_tokenIssuerSettings);

        var response = await _httpRequester.DeleteAsync<IntegrationServiceResponse>(
            $"{apiGatewayUrl}/api/payments/{paymentId}",
            request,
            tokenResponse.AccessToken);

        if (!response.Success)
            throw new Exception(response.Message);

        return response;
    }

    public async Task<IntegrationServiceResponse> RequestPayment(string apiGatewayUrl, PaymentRequest request)
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationToken(_tokenIssuerSettings);

        var response = await _httpRequester.PostAsync<IntegrationServiceResponse>(
            $"{apiGatewayUrl}/api/payments",
            request,
            tokenResponse.AccessToken);

        if (!response.Success)
            throw new Exception(response.Message);

        return response;
    }
}