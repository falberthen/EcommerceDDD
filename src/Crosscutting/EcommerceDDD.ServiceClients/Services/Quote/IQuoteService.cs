using EcommerceDDD.ServiceClients.QuoteManagement.Models;

namespace EcommerceDDD.ServiceClients.Services.Quote;

public interface IQuoteService
{
    Task<QuoteViewModel?> GetQuoteDetailsAsync(Guid quoteId, CancellationToken cancellationToken);
    Task ConfirmQuoteAsync(Guid quoteId, CancellationToken cancellationToken);
}
