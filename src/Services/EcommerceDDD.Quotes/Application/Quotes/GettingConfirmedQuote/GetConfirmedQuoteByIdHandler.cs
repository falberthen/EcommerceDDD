namespace EcommerceDDD.Quotes.Application.Quotes.GettingConfirmedQuote;

public class GetConfirmedQuoteByIdHandler : IQueryHandler<GetConfirmedQuoteById, QuoteViewModel>
{
    private readonly IQuerySession _querySession;
    
    public GetConfirmedQuoteByIdHandler(IQuerySession querySession)
    {       
        _querySession = querySession;
    }

    public Task<QuoteViewModel> Handle(GetConfirmedQuoteById query, CancellationToken cancellationToken)
    {
        var projectedQuote = _querySession.Query<QuoteDetails>()
            .FirstOrDefault(q => q.Id == query.QuoteId.Value 
            && q.QuoteStatus == QuoteStatus.Confirmed);

        QuoteViewModel viewModel = default!;

        if (projectedQuote is not null)
        {
            viewModel = new QuoteViewModel() with
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
                var producIds = projectedQuote.Items
                    .Select(pid => pid.ProductId)
                    .ToArray();

                var quoteItems = projectedQuote.Items.Select(item => new QuoteItemViewModel()
                {
                    ProductId = item.ProductId,                     
                    Quantity = item.Quantity
                }).ToList();

                viewModel = viewModel with { Items = quoteItems };
                viewModel.CurrencySymbol = currency.Symbol;
                viewModel.CurrencyCode = currency.Code;
            }
        }

        return Task.FromResult(viewModel);
    }
}