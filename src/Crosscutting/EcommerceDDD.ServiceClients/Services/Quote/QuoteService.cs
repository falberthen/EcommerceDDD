namespace EcommerceDDD.ServiceClients.Services.Quote;

public class QuoteService(QuoteManagementClient quoteManagementClient) : IQuoteService
{
    private readonly QuoteManagementClient _quoteManagementClient = quoteManagementClient;

    public async Task<QuoteViewModel?> GetQuoteDetailsAsync(Guid quoteId, CancellationToken cancellationToken)
    {
        return await _quoteManagementClient.Api.V2.Internal.Quotes[quoteId].Details
            .GetAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> ConfirmQuoteAsync(Guid quoteId, Guid orderId, CancellationToken cancellationToken)
    {
        var request = new ConfirmQuoteRequest()
        {
            OrderId = orderId
        };

        try
        {
            await _quoteManagementClient.Api.V2.Internal.Quotes[quoteId].Confirm
                .PutAsync(request, cancellationToken: cancellationToken);
            return true;
        }
        // 422 is the quote's business rule (taken by another order or cancelled), not a technical failure to retry.
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.UnprocessableEntity)
        {
            return false;
        }
    }
}
