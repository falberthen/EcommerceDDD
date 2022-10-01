namespace EcommerceDDD.Orders.Domain;

public sealed class ProductId : StronglyTypedId<Guid>
{
    public static ProductId Of(Guid value)
    {
        return new ProductId(value);
    }

    public ProductId(Guid value) : base(value)
    {
    }
}
