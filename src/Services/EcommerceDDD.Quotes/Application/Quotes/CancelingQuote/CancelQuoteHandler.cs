using MediatR;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Quotes.Domain.Commands;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Quotes.Application.Quotes.CancelingQuote;

public class CancelQuoteHandler : ICommandHandler<CancelQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;

    public CancelQuoteHandler(IEventStoreRepository<Quote> quoteWriteRepository)
    {
        _quoteWriteRepository = quoteWriteRepository;
    }

    public async Task Handle(CancelQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStreamAsync(command.QuoteId.Value)
            ?? throw new RecordNotFoundException("The quote was not found.");
     
        quote.Cancel();

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}