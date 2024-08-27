namespace EcommerceDDD.OrderProcessing.Domain;

public sealed class ProductId(Guid value) : StronglyTypedId<Guid>(value)
{
    public static ProductId Of(Guid value) => new ProductId(value);
}
