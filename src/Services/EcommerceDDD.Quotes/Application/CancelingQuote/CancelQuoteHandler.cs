using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Application.Quotes.CancelingQuote;

public class CancelQuoteHandler : CommandHandler<CancelQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;

    public CancelQuoteHandler(IEventStoreRepository<Quote> quoteWriteRepository)
    {
        _quoteWriteRepository = quoteWriteRepository;
    }

    public override async Task Handle(CancelQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStream(command.QuoteId.Value);

        if (quote == null)
            throw new ApplicationException("The quote was not found.");
     
        quote.Cancel();

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}