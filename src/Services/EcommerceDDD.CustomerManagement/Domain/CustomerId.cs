namespace EcommerceDDD.CustomerManagement.Domain;

public sealed class CustomerId : StronglyTypedId<Guid>
{
    public CustomerId(Guid value) : base(value)
    {
    }

    public static CustomerId Of(Guid value)
    {
        return new CustomerId(value);
    }
}
