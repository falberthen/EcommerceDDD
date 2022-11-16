using MediatR;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Quotes.Domain.Commands;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Quotes.Application.Quotes.ConfirmingQuote;

public class ConfirmQuoteHandler : ICommandHandler<ConfirmQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;

    public ConfirmQuoteHandler(        
        IEventStoreRepository<Quote> quoteWriteRepository)
    {
        _quoteWriteRepository = quoteWriteRepository;
    }

    public async Task<Unit> Handle(ConfirmQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStreamAsync(command.QuoteId.Value)
            ?? throw new RecordNotFoundException("Quote not found.");
        
        // Quote confirmed
        quote.Confirm(command.Currency);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);

        return Unit.Value;
    }
}