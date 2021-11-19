using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Quotes;

public class QuoteId : StronglyTypedId<QuoteId>
{
    public QuoteId(Guid value) : base(value)
    {
    }

    public static QuoteId Of(Guid quoteId)
    {
        if (quoteId == Guid.Empty)
            throw new BusinessRuleException("Quote Id must be provided.");

        return new QuoteId(quoteId);
    }
}