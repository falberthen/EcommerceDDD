namespace EcommerceDDD.CustomerManagement.Domain;

public sealed class CustomerId(Guid value) : StronglyTypedId<Guid>(value)
{
    public static CustomerId Of(Guid value) => new CustomerId(value);
}
