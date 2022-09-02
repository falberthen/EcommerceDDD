namespace EcommerceDDD.Products.Domain;

public sealed class ProductId : StronglyTypedId<Guid>
{
    public static ProductId Of(Guid value)
    {
        return new ProductId(value);
    }

    public static IEnumerable<ProductId> Of(IList<Guid> values)
    {
        foreach (var item in values)
            yield return Of(item);
    }

    public ProductId(Guid value) : base(value)
    {
    }
}
