namespace EcommerceDDD.InventoryManagement.Domain;

public sealed class ProductId : StronglyTypedId<Guid>
{
    public static ProductId Of(Guid value)
    {
        return new ProductId(value);
    }

    public ProductId(Guid value) : base(value)
    {
    }

    public static IEnumerable<ProductId> Of(IList<Guid> values)
    {
        foreach (var item in values)
            yield return Of(item);
    }
}
