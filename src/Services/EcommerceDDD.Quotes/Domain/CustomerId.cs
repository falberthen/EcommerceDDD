namespace EcommerceDDD.Quotes.Domain;

public sealed class CustomerId : StronglyTypedId<Guid>
{
    public static CustomerId Of(Guid value)
    {
        return new CustomerId(value);
    }

    public CustomerId(Guid value) : base(value)
    {
    }
}
