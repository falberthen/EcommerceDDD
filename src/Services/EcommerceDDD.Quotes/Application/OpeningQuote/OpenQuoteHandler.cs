using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Persistence;

namespace EcommerceDDD.Quotes.Application.OpeningQuote;

public class OpenQuoteHandler : CommandHandler<OpenQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;
    private readonly ICustomerQuoteOpennessChecker _checker;

    public OpenQuoteHandler(IEventStoreRepository<Quote> quoteWriteRepository,
        ICustomerQuoteOpennessChecker checker)
    {
        _quoteWriteRepository = quoteWriteRepository;
        _checker = checker;
    }

    public override async Task Handle(OpenQuote command, CancellationToken cancellationToken)
    {
        var quote = await Quote
            .CreateNew(command.CustomerId, _checker);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}