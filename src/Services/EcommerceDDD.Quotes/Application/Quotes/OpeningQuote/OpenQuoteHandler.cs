using MediatR;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Quotes.Domain.Commands;

namespace EcommerceDDD.Quotes.Application.Quotes.OpeningQuote;

public class OpenQuoteHandler : ICommandHandler<OpenQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;
    private readonly ICustomerOpenQuoteChecker _customerOpenQuoteChecker;

    public OpenQuoteHandler(IEventStoreRepository<Quote> quoteWriteRepository,
        ICustomerOpenQuoteChecker customerOpenQuoteChecker)
    {
        _quoteWriteRepository = quoteWriteRepository;
        _customerOpenQuoteChecker = customerOpenQuoteChecker;
    }

    public async Task Handle(OpenQuote command, CancellationToken cancellationToken)
    {
        if (await _customerOpenQuoteChecker.CustomerHasOpenQuote(command.CustomerId))
            throw new ApplicationLogicException("The customer has an open quote already.");

        var quote = Quote.Create(command.CustomerId);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}