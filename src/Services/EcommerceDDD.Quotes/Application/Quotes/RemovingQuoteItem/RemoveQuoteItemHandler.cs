using MediatR;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Quotes.Domain.Commands;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Quotes.Application.Quotes.RemovingQuoteItem;

public class RemoveQuoteItemHandler : ICommandHandler<RemoveQuoteItem>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;

    public RemoveQuoteItemHandler(IEventStoreRepository<Quote> quoteWriteRepository)
    {
        _quoteWriteRepository = quoteWriteRepository;
    }

    public async Task<Unit> Handle(RemoveQuoteItem command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStreamAsync(command.QuoteId.Value)
            ?? throw new RecordNotFoundException("Quote not found.");
     
        quote.RemoveItem(command.ProductId);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);

        return Unit.Value;
    }
}