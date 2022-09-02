using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Application.Quotes.RemovingQuoteItem;

namespace EcommerceDDD.Quotes.Application.RemovingQuoteItem;

public class RemoveQuoteItemHandler : CommandHandler<RemoveQuoteItem>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;

    public RemoveQuoteItemHandler(IEventStoreRepository<Quote> quoteWriteRepository)
    {
        _quoteWriteRepository = quoteWriteRepository;
    }

    public async override Task Handle(RemoveQuoteItem command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStream(command.QuoteId.Value);

        if (quote == null)
            throw new ApplicationException("The quote was not found.");
     
        quote.RemoveQuoteItem(command.ProductId);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);

        await Task.CompletedTask;
    }
}