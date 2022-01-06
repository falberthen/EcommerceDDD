using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Customers;

public sealed class CustomerId : StronglyTypedId<CustomerId>
{
    public CustomerId(Guid value) : base(value)
    {
    }
}