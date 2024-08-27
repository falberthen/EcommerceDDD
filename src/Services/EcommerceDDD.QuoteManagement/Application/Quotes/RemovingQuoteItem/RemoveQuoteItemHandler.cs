using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EcommerceDDD.QuoteManagement.Application.Quotes.RemovingQuoteItem;

public class RemoveQuoteItemHandler(IEventStoreRepository<Quote> quoteWriteRepository) : ICommandHandler<RemoveQuoteItem>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;

    public async Task Handle(RemoveQuoteItem command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
        .FetchStreamAsync(command.QuoteId.Value)
            ?? throw new RecordNotFoundException($"The quote {command.QuoteId} not found.");

        quote.RemoveItem(command.ProductId);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}