namespace EcommerceDDD.Quotes.Application.Quotes.GettingConfirmedQuote;

public class GetConfirmedQuoteByIdHandler(IQuerySession querySession) : IQueryHandler<GetConfirmedQuoteById, QuoteViewModel>
{
    private readonly IQuerySession _querySession = querySession;

    public Task<Result<QuoteViewModel>> HandleAsync(GetConfirmedQuoteById query, CancellationToken cancellationToken)
    {
        var projectedQuote = _querySession.Query<QuoteDetails>()
            .FirstOrDefault(q => q.Id == query.QuoteId.Value
            && q.QuoteStatus == QuoteStatus.Confirmed);

        if (projectedQuote is null)
            return Task.FromResult(Result.Fail<QuoteViewModel>($"The quote {query.QuoteId} not found."));

        QuoteViewModel viewModel = new QuoteViewModel() with
        {
            QuoteId = projectedQuote.Id,
            CustomerId = projectedQuote.CustomerId,
            CreatedAt = projectedQuote.CreatedAt,
            QuoteStatus = projectedQuote.QuoteStatus.ToString(),
            Items = new List<QuoteItemViewModel>()
        };

        var currency = Currency.OfCode(projectedQuote.CurrencyCode);

        if (projectedQuote.Items is not null
            && projectedQuote.Items.Count > 0)
        {
            var quoteItems = projectedQuote.Items.Select(item => new QuoteItemViewModel()
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();

            viewModel = viewModel with { Items = quoteItems };
            viewModel.CurrencySymbol = currency.Symbol;
            viewModel.CurrencyCode = currency.Code;
        }

        return Task.FromResult(Result.Ok(viewModel));
    }
}
