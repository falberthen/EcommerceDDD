namespace EcommerceDDD.ServiceClients.Services.Quote;

public interface IQuoteService
{
    Task<QuoteViewModel?> GetQuoteDetailsAsync(Guid quoteId, CancellationToken cancellationToken);
    Task<bool> ConfirmQuoteAsync(Guid quoteId, Guid orderId, CancellationToken cancellationToken);
}
