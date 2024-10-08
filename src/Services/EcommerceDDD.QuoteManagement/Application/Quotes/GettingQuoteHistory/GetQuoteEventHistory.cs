﻿namespace EcommerceDDD.QuoteManagement.Application.Quotes.GettingQuoteHistory;

public record class GetQuoteEventHistory : IQuery<IList<QuoteEventHistory>>
{
    public QuoteId QuoteId { get; private set; }

    public static GetQuoteEventHistory Create(QuoteId quoteId)
    {
        if (quoteId is null)
            throw new ArgumentNullException(nameof(quoteId));

        return new GetQuoteEventHistory(quoteId);
    }

    private GetQuoteEventHistory(QuoteId quoteId) => QuoteId = quoteId;
}