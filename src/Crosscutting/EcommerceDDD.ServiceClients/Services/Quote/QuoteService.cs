using EcommerceDDD.ServiceClients.QuoteManagement;
using EcommerceDDD.ServiceClients.QuoteManagement.Models;

namespace EcommerceDDD.ServiceClients.Services.Quote;

public class QuoteService(QuoteManagementClient quoteManagementClient) : IQuoteService
{
    private readonly QuoteManagementClient _quoteManagementClient = quoteManagementClient;

    public async Task<QuoteViewModel?> GetQuoteDetailsAsync(Guid quoteId, CancellationToken cancellationToken)
    {
        return await _quoteManagementClient.Api.V2.Internal.Quotes[quoteId].Details
            .GetAsync(cancellationToken: cancellationToken);
    }

    public async Task ConfirmQuoteAsync(Guid quoteId, CancellationToken cancellationToken)
    {
        await _quoteManagementClient.Api.V2.Internal.Quotes[quoteId].Confirm
            .PutAsync(cancellationToken: cancellationToken);
    }
}
