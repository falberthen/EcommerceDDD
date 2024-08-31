namespace EcommerceDDD.OrderProcessing.Domain;

public sealed class QuoteId(Guid value) : StronglyTypedId<Guid>(value)
{
    public static QuoteId Of(Guid value) => new QuoteId(value);
}