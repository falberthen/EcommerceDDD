using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Quotes;

public sealed class QuoteId : StronglyTypedId<QuoteId>
{
    public QuoteId(Guid value) : base(value)
    {
    }
}