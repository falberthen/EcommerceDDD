namespace EcommerceDDD.Orders.Domain;

public sealed class QuoteId : StronglyTypedId<Guid>
{
    public static QuoteId Of(Guid value)
    {
        return new QuoteId(value);
    }

    public QuoteId(Guid value) : base(value)
    {
    }
}