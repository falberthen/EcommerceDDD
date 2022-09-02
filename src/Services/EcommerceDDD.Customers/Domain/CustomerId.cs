using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Customers.Domain;

public sealed class CustomerId : StronglyTypedId<Guid>
{
    public CustomerId(Guid value) : base(value)
    {
    }

    public static CustomerId Create()
    {
        return new CustomerId(Guid.NewGuid());
    }
}
