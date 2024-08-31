namespace EcommerceDDD.InventoryManagement.Domain;

public sealed class ProductId(Guid value) : StronglyTypedId<Guid>(value)
{
    public static ProductId Of(Guid value) => new ProductId(value);

    public static IEnumerable<ProductId> Of(IList<Guid> values)
    {
        foreach (var item in values)
            yield return Of(item);
    }
}
